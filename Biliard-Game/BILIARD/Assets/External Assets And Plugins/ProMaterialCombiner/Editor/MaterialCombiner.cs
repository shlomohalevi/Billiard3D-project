/*
  Created by:
  Juan Sebastian Munoz
  naruse@gmail.com
  All rights reserved

 */
namespace ProMaterialCombiner {
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;

	public class MaterialCombiner {
	    private List<Texture2D> texturesToAtlas;
	    private List<Vector2> scales;//used for tiling of the textures
	    private List<Vector2> offsets;//used for tiling of the textures.

        private GameObject objToCombine;

        private Material[] objectMaterials;
        public Material[] ObjectMaterials { get { return objectMaterials; } }

        private string shaderToCombine = "";

        //obj *should* be assembled correctly, supposes the object is correct.
        public MaterialCombiner(GameObject obj, bool usesSkinnedMeshRenderer) {
            objToCombine = obj;
            objectMaterials = usesSkinnedMeshRenderer ? obj.GetComponent<SkinnedMeshRenderer>().sharedMaterials : obj.GetComponent<MeshRenderer>().sharedMaterials;
            shaderToCombine = GetShaderName(objToCombine);
        }

	    //(objToCombine should be correctly assembled,meaning has MeshRenderer,filter OR SkinnedMeshRenderer) AND and shares the same type of shader across materials
	    //combines the number of materials in the mesh renderer, not the submesh count.
        public GameObject CombineMaterials(bool usesSkinnedMeshRenderer, string customAtlasName, bool generatePrefabs) {
            // // // when generating prefabs // // //
            string folderToSavePrefabs =
                 #if UNITY_5_4_OR_NEWER
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
                #else
                    EditorApplication.currentScene;
                #endif

            if(generatePrefabs) {
                if(folderToSavePrefabs == "") { //scene is not saved yet.
                    folderToSavePrefabs = Constants.NonSavedSceneFolderName + ".unity";
                }
                folderToSavePrefabs = folderToSavePrefabs.Substring(0, folderToSavePrefabs.Length-6) + "-Atlas";//remove the ".unity"
                folderToSavePrefabs += Path.DirectorySeparatorChar + "Prefabs";
                if(!Directory.Exists(folderToSavePrefabs)) {
                    Directory.CreateDirectory(folderToSavePrefabs);
                    AssetDatabase.Refresh();
                }
            }
            ///////////////////////////////////////////

	        List<string> shaderDefines = ShaderManager.Instance.GetShaderTexturesDefines(shaderToCombine);
            Debug.Log(shaderDefines.Count + " " + shaderToCombine);

	        int atlasSize = GetAproxAtlasSize(objectMaterials[0]);
	        Atlasser atlas = new Atlasser(atlasSize, atlasSize);
	        // generate atlas for the initial textures
	        int resizeTimes = 1;

            TextureReuseManager textureReuseManager = new TextureReuseManager();
	        GetTexturesScalesAndOffsetsForShaderDefine(objectMaterials, shaderDefines[0]);//objectMaterials has same length than texturesToAtlas.

            Node resultNode = null;

            for(int i = 0; i < texturesToAtlas.Count; i++) {
                resultNode = atlas.Insert(texturesToAtlas[i].width, texturesToAtlas[i].height);

                if(resultNode == null) {
                    int resizedAtlasSize = atlasSize + Mathf.RoundToInt((float)atlasSize * Constants.AtlasResizeFactor * resizeTimes);
                    atlas = new Atlasser(resizedAtlasSize, resizedAtlasSize);
                    i = -1;//at the end of the loop 1 will be added and it will start in 0
                    textureReuseManager.ClearTextureRefs();
                    resizeTimes++;
                }
            }

            string timeStamp = Utils.GenerateTimeStamp();//this is used to generate unique prefabs/materials/etc so we dont overwrite already created materials

	        //with the generated atlas, save the textures and load them and add them to the combinedMaterial
	        string pathToAtlas = CreateFolderForCombinedObject(objToCombine);
	        string fileName = ((customAtlasName == "") ? "MaterialAtlas " : customAtlasName + " ") + shaderToCombine.Replace('/','_');
	        string atlasTexturePath = pathToAtlas + Path.DirectorySeparatorChar + fileName;

	        //create material and fill with the combined to be textures in the material
            string shaderMaterialName = shaderToCombine;
            if(Utils.IsShaderStandard(shaderMaterialName))
                shaderMaterialName = Utils.ExtractStandardShaderOriginalName(shaderMaterialName);
	        Material combinedMaterial = new Material(Shader.Find(shaderMaterialName));

	        AssetDatabase.CreateAsset(combinedMaterial, atlasTexturePath + timeStamp + "Mat.mat");
            AssetDatabase.ImportAsset(atlasTexturePath + timeStamp + "Mat.mat");
	        //AssetDatabase.Refresh();
	        combinedMaterial = (Material) AssetDatabase.LoadAssetAtPath(atlasTexturePath + timeStamp + "Mat.mat", typeof(Material));

	        for(int i = 0; i < shaderDefines.Count; i++) {
	            GetTexturesScalesAndOffsetsForShaderDefine(objectMaterials, shaderDefines[i]);

				atlas.SaveAtlasToFile(atlasTexturePath + i.ToString() + timeStamp + ".png", texturesToAtlas, scales, offsets);
                AssetDatabase.ImportAsset(atlasTexturePath + i.ToString() + timeStamp + ".png");

	            Texture2D savedAtlasTexture = (Texture2D) AssetDatabase.LoadAssetAtPath(atlasTexturePath + i.ToString() + timeStamp + ".png", typeof(Texture2D));
	            combinedMaterial.SetTexture(shaderDefines[i], savedAtlasTexture);
	        }

	        Mesh masterMesh = usesSkinnedMeshRenderer ? objToCombine.GetComponent<SkinnedMeshRenderer>().sharedMesh : objToCombine.GetComponent<MeshFilter>().sharedMesh;
	        Mesh[] subMeshes = new Mesh[objectMaterials.Length];
	        for(int i = 0; i < subMeshes.Length; i++) {
	            subMeshes[i] = ExtractMesh(masterMesh, i);
	            Vector2[] remappedUVs = subMeshes[i].uv;
                bool generatedTexture = (objectMaterials[i].mainTexture == null);
	            for(int j = 0; j < remappedUVs.Length; j++) {
                    remappedUVs[j] = Utils.ReMapUV(remappedUVs[j], atlas.AtlasWidth, atlas.AtlasHeight, atlas.TexturePositions[i], objToCombine.name, generatedTexture);
	            }
	            subMeshes[i].uv = remappedUVs;
	        }
	        GameObject combinedObj = GameObject.Instantiate(objToCombine,
                                                            objToCombine.transform.position,
                                                            objToCombine.transform.rotation) as GameObject;
            if(usesSkinnedMeshRenderer) {
                combinedObj.GetComponent<SkinnedMeshRenderer>().sharedMaterials = new Material[] { combinedMaterial };
                combinedObj.GetComponent<SkinnedMeshRenderer>().sharedMesh = Utils.CombineMeshes(subMeshes);
                combinedObj.GetComponent<SkinnedMeshRenderer>().sharedMesh.boneWeights = objToCombine.GetComponent<SkinnedMeshRenderer>().sharedMesh.boneWeights;
                combinedObj.GetComponent<SkinnedMeshRenderer>().sharedMesh.bindposes = objToCombine.GetComponent<SkinnedMeshRenderer>().sharedMesh.bindposes;
            } else {
                combinedObj.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { combinedMaterial };
                combinedObj.GetComponent<MeshFilter>().sharedMesh = Utils.CombineMeshes(subMeshes);
            }
            combinedObj.transform.parent = objToCombine.transform.parent;
            combinedObj.transform.localScale = objToCombine.transform.localScale;
	        combinedObj.name = objToCombine.name + Constants.OptimizedObjIdentifier;

            if(generatePrefabs) {
                string prefabName = Utils.GetValidName(combinedObj.name);
                string  assetPath = folderToSavePrefabs + Path.DirectorySeparatorChar + prefabName;
                AssetDatabase.CreateAsset(usesSkinnedMeshRenderer ? combinedObj.GetComponent<SkinnedMeshRenderer>().sharedMesh : combinedObj.GetComponent<MeshFilter>().sharedMesh,
                                          assetPath + timeStamp +".asset");
				#if UNITY_EDITOR_OSX
	            PrefabUtility.SaveAsPrefabAssetAndConnect(combinedObj, assetPath + timeStamp + ".prefab", InteractionMode.AutomatedAction);
                	//PrefabUtility.CreatePrefab(assetPath + timeStamp +".prefab", combinedObj, ReplacePrefabOptions.ConnectToPrefab);
				#elif UNITY_EDITOR_WIN
					assetPath = assetPath.Replace("\\", "/");//this has to be done as it seems EditorApplication.CurrentScene creates conflicts with "\'s"
					PrefabUtility.SaveAsPrefabAssetAndConnect(combinedObj, assetPath + timeStamp + ".prefab", InteractionMode.AutomatedAction);
				#endif
            }
	        return combinedObj;
	    }

	    //creates a folder where the scene resides and then creates a subfolder with the obj name and InstanceID
	    //returns the created path
	    private string CreateFolderForCombinedObject(GameObject g) {
			string folderToSaveAssets =
                #if UNITY_5_4_OR_NEWER
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
                #else
                    EditorApplication.currentScene;
                #endif
			if(folderToSaveAssets == "") { //scene is not saved yet.
				folderToSaveAssets = Constants.NonSavedSceneFolderName + ".unity";
				Debug.LogWarning("WARNING: Scene has not been saved, saving baked objects to: " + Constants.NonSavedSceneFolderName + " folder");
			}
	        string path = folderToSaveAssets.Substring(0, folderToSaveAssets.Length-6) + "-Atlas";//rm .unity
	        if(!Directory.Exists(path)) {
	            Directory.CreateDirectory(path);
	            AssetDatabase.ImportAsset(path);
                //AssetDatabase.Refresh();
	        }
	        //create specific directory for the combined obj
	        path += Path.DirectorySeparatorChar + g.name + g.GetInstanceID();
	        if(!Directory.Exists(path)) {
	            Directory.CreateDirectory(path);
	            AssetDatabase.ImportAsset(path);
                //AssetDatabase.Refresh();
	        }
	        return path;
	    }


	    private void GetTexturesScalesAndOffsetsForShaderDefine(Material[] materials, string shaderDefine) {
	        //Material[] materials = obj.GetComponent<MeshRenderer>().sharedMaterials;
	        texturesToAtlas = new List<Texture2D>();
	        scales = new List<Vector2>();
	        offsets = new List<Vector2>();

	        for(int i = 0; i < materials.Length; i++) {
	            if(materials[i] != null) {
	                Texture2D extractedTexture = materials[i].GetTexture(shaderDefine) as Texture2D;
	                if(extractedTexture) {
	                    texturesToAtlas.Add(extractedTexture);
	                    scales.Add(materials[i].GetTextureScale(shaderDefine));
	                    offsets.Add(materials[i].GetTextureOffset(shaderDefine));
	                } else {//material doesnt have a texture with that define/there is no texture.lets generate a texture with the color.
	                    if(materials[i].HasProperty("_Color"))//check if mat has a color property
	                        texturesToAtlas.Add(Utils.GenerateTexture(materials[i].GetColor("_Color")));
	                    else
	                        texturesToAtlas.Add(Utils.GenerateTexture(Color.white));
	                    scales.Add(Vector2.one);
	                    offsets.Add(Vector2.zero);
	                }
	            } else {
	                //null material, generate a white texture.
	                texturesToAtlas.Add(Utils.GenerateTexture(Color.white));
	                scales.Add(Vector2.one);
	                offsets.Add(Vector2.zero);
	            }
	        }
	    }

	    public int GetAproxAtlasSize(Material mat) {
	        int atlasSize = 0;

            List <string> shaderDefines;

            if(Utils.IsShaderStandard(shaderToCombine)) {
                shaderDefines = ShaderManager.Instance.GetShaderTexturesDefines(Utils.ExtractStandardShaderOriginalName(shaderToCombine), false, mat);
            } else {
                shaderDefines = ShaderManager.Instance.GetShaderTexturesDefines(shaderToCombine);
            }

            if(shaderToCombine == "" || shaderDefines == null) //the game object is not supported || shader is not recognized
                return 0;

            string shaderTextureDefine = shaderDefines[0];

            for(int i = 0; i < objectMaterials.Length; i++) {
                if(objectMaterials[i] != null) {
                    Texture2D refTexture = objectMaterials[i].GetTexture(shaderTextureDefine) as Texture2D;
                    if(refTexture != null)
                        atlasSize += (refTexture.width * refTexture.height);
                    else
                        atlasSize += (Constants.NullTextureSize * Constants.NullTextureSize);
                }
            }
	        return Mathf.RoundToInt(Mathf.Sqrt(atlasSize));
	    }


	    private Mesh ExtractMesh(Mesh masterMesh, int subMeshToExtract) {
	        Dictionary<int, int> indexMap = new Dictionary<int, int>();
	        int[] meshIndices = masterMesh.GetIndices(subMeshToExtract);
	        int counter = 0;
	        //get unique indexes
	        for(int i = 0; i < meshIndices.Length; i++) {
	            if(!indexMap.ContainsKey(meshIndices[i])) {
	                indexMap.Add(meshIndices[i], counter);
	                counter++;
	            }/* else {
	                Debug.LogError("Index exists already! " + meshIndices[i]);
	            }*/
	        }

	        List<Vector3> extractedMeshVertices = new List<Vector3>();
	        List<Vector2> extractedMeshUvs = new List<Vector2>();
	        List<Vector3> extractedMeshNormals = new List<Vector3>();
	        //start filling the vertices,uvs and normals for the acquired indexes
	        foreach(KeyValuePair<int, int> pair in indexMap) {
	            extractedMeshVertices.Add(masterMesh.vertices[pair.Key]);
	            extractedMeshUvs.Add(masterMesh.uv[pair.Key]);
	            extractedMeshNormals.Add(masterMesh.normals[pair.Key]);
	        }

	        int[] subMeshTriangles = masterMesh.GetTriangles(subMeshToExtract);
	        int[] extractedMeshTriangles = new int[subMeshTriangles.Length];
	        for(int i = 0; i < subMeshTriangles.Length; i++) {
	            extractedMeshTriangles[i] = indexMap[subMeshTriangles[i]];
	        }
	        Mesh extractedMesh = new Mesh();
	        extractedMesh.vertices = extractedMeshVertices.ToArray();
	        extractedMesh.uv = extractedMeshUvs.ToArray();
	        extractedMesh.normals = extractedMeshNormals.ToArray();
	        extractedMesh.triangles = extractedMeshTriangles;

	        return extractedMesh;
	    }
        private string GetShaderName(GameObject g) {
            string shaderName = g.GetComponent<Renderer>().sharedMaterials[0].shader.name;
            if(Utils.IsShaderStandard(shaderName))
                shaderName = Utils.ParseStandardShaderName(g.GetComponent<Renderer>().sharedMaterials[0]);
            return shaderName;
	    }
	}
}