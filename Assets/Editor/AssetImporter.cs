using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR

using UnityEditor;

namespace Tool
{


    public class CustomEditorWindow : EditorWindow
    {
        string sourcePath = "";
        string importToFolder = CustomAssetImporter.DefaultPath;
        string fileExtention = "png";
        string nameModifier = "";

        bool overwriteExisting = false;
        bool isSpritesheet = false;

        int sprites = 4;

        int sizeX = 32;
        int sizeY = 32;

        float paddingX = 0;
        float paddingY = 0;

        float offsetX = 0;
        float offsetY = 0;

        [MenuItem("Tool/Asset Importer")]
        public static void ShowWindow()
        {

            var window = GetWindow<CustomEditorWindow>("Asset Importer", true);

            window.minSize = new Vector2(200, 200);
            window.Show();
        }


        private void OnGUI()
        {

            sourcePath = EditorGUILayout.TextField(new GUIContent("File Path:"), sourcePath);
            if (GUILayout.Button("Browse"))
            {
                 sourcePath = EditorUtility.OpenFolderPanel("Assets to import", "", "");
            }

            importToFolder = EditorGUILayout.TextField(new GUIContent("Import to folder:"), importToFolder);
            fileExtention = EditorGUILayout.TextField(new GUIContent("File Type: (blank for any)"), fileExtention);
         //   nameModifier = EditorGUILayout.TextField(new GUIContent("Name modifier:"), nameModifier);

            overwriteExisting = EditorGUILayout.Toggle("Overwite existing files", overwriteExisting);
            isSpritesheet = EditorGUILayout.Toggle("sprite sheet", isSpritesheet);

            if (isSpritesheet)
            {
                sprites = EditorGUILayout.IntField(new GUIContent("Sprites :"), sprites);
                sizeX = EditorGUILayout.IntField(new GUIContent("Size X:"), sizeX);
                sizeY = EditorGUILayout.IntField(new GUIContent("Size Y:"), sizeY);
                paddingX = EditorGUILayout.FloatField(new GUIContent("Padding X:"), paddingX);
                paddingY = EditorGUILayout.FloatField(new GUIContent("Padding Y:"), paddingY);
                offsetX = EditorGUILayout.FloatField(new GUIContent("Offset X:"), offsetX);
                offsetY = EditorGUILayout.FloatField(new GUIContent("Offset Y:"), offsetY);

            }

            if (GUILayout.Button("Import"))
            {
                AssetImportPostProcessing.SetUp(nameModifier, isSpritesheet, sprites, sizeX, sizeY, paddingX, paddingY, offsetX, offsetY);
                CustomAssetImporter.CustomImportAsset(sourcePath, importToFolder, fileExtention, overwriteExisting);
            }
        }
    }

    public class CustomAssetImporter
    {

        public const string DefaultPath = "_Imported Assets";

        public static void CustomImportAsset(string sourceDirectoryPath, string outputDirectory, string filetype, bool overwriteExisting)
        {
            AssetImportPostProcessing.settingsEnabled = true;


            outputDirectory = !(outputDirectory == null || outputDirectory == "") ? outputDirectory : DefaultPath;
            filetype = !(filetype == null || filetype == "") ? filetype : "*";


            try
            {
                AssetDatabase.StartAssetEditing();

                // get files in dir
                string[] foundFiles = Directory.GetFiles(sourceDirectoryPath, $"*.{filetype}");
                if (foundFiles.Length == 0)
                {
                    throw new System.Exception($"There are no files in path <{sourceDirectoryPath}> of type <{filetype}>");
                }

                var localPath = Path.Combine("Assets", outputDirectory);


                bool pathExists = Directory.Exists(localPath);

                if (!pathExists)
                {
                    Debug.Log($"Directory at {outputDirectory} does not exist, creating {localPath}");
                    if (!Directory.CreateDirectory(localPath).Exists)
                        throw new System.Exception($"Could not create directory at {localPath}");
                }
                int count = 0;

                // import each files
                foreach (string sourcePath in foundFiles)
                {
                    var assetPath = ImportFile(overwriteExisting, localPath, sourcePath);

                    if (assetPath == null) 
                        continue;

                    count++;

               
                }

                Debug.Log($"{count} assets imported sucessfully");

            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
                AssetImportPostProcessing.settingsEnabled = false;
            }

        }

        private static string ImportFile(bool overwriteExisting, string localPath, string sourcePath)
        {
            var sourceFile = new FileInfo(sourcePath);

  
            if (!sourceFile.Exists)
            {
                Debug.LogError($"Specified file at {sourcePath} does not exist or cannot be read");
                return null;
            }

            string assetPath = Path.Combine(localPath, Path.GetFileName(sourcePath));

            sourceFile.CopyTo(assetPath, overwriteExisting);

            AssetDatabase.ImportAsset(assetPath);

            return assetPath;
        }
    }



    public class AssetImportPostProcessing : AssetPostprocessor
    {
        public static bool settingsEnabled = false;

        public static string nameModifier;
        public static bool isSpriteSheet;
        public static int sprites;
        public static int spriteSizeX = 32;
        public static int spriteSizeY = 32;
        public static float spritePaddingY = 0;
        public static float spritePaddingX = 0;
        public static float spriteoffsetY = 0;
        public static float spriteOffsetX = 0;
        public static TextureImporterType textureImportType = TextureImporterType.Sprite;
        public static int pixelsPerUnit = 32;
        public static FilterMode filterMode = FilterMode.Point;
        public static TextureImporterCompression compressionMode = TextureImporterCompression.Uncompressed;

        internal static void SetUp(string nameModifier, bool isSpritesheet, int sprites, int sizeX, int sizeY, float paddingX, float paddingY, float offsetX, float offsetY)
        {
            AssetImportPostProcessing.nameModifier = nameModifier;

            AssetImportPostProcessing.isSpriteSheet = isSpritesheet;

            AssetImportPostProcessing.sprites = sprites;

            AssetImportPostProcessing.spriteSizeX = sizeX;
            AssetImportPostProcessing.spriteSizeY = sizeY;
            AssetImportPostProcessing.spritePaddingX = paddingX;
            AssetImportPostProcessing.spritePaddingY = paddingY;
            AssetImportPostProcessing.spriteoffsetY = offsetX;
            AssetImportPostProcessing.spriteOffsetX = offsetY;
        }

        void OnPostprocessSprites(Texture2D texture, Sprite[] s)
        {
            if (!settingsEnabled)
            {
                return;
            }

            var importer = assetImporter as TextureImporter;
        //    importer.assetBundleName = $"{importer.assetBundleName}{(nameModifier == "" ? "" : $"_{nameModifier}")}";
            importer.textureType = textureImportType;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.filterMode = filterMode;
            importer.textureCompression = compressionMode;

            if (isSpriteSheet)
            {
                Debug.Log("Sprite Sheeting");

                importer.spriteImportMode = SpriteImportMode.Multiple;

                SpriteMetaData[] spritesheet = new SpriteMetaData[sprites];

                for (int i = 0; i < sprites; i++)
                {
                    SpriteMetaData sprite = new SpriteMetaData();


                    sprite.name = $"{importer.assetBundleName}_s_{i.ToString("00")}";
                    
                    sprite.rect = new Rect(spriteOffsetX + i * (spriteSizeX + spritePaddingX), spriteoffsetY+ spritePaddingY, spriteSizeX, spriteSizeY);

                    spritesheet[i] = sprite;
                }
                Debug.Log(spritesheet.Length);
                importer.spritesheet = spritesheet;
            }


            importer.SaveAndReimport();
                
            Debug.Log("Import settings applied");
           
        }



    }
}
#endif