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

        bool overwriteExisting = false;
        bool isSpritesheet = false;

        float paddingX = 8;
        float paddingY = 8;

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

            overwriteExisting = EditorGUILayout.Toggle("Overwite existing files", overwriteExisting);
            isSpritesheet = EditorGUILayout.Toggle("sprite sheet", isSpritesheet);

            if (isSpritesheet)
            {
                paddingX = EditorGUILayout.FloatField(new GUIContent("Padding X:"), paddingX);
                paddingY = EditorGUILayout.FloatField(new GUIContent("Padding Y:"), paddingY);
                offsetX = EditorGUILayout.FloatField(new GUIContent("Offset X:"), offsetX);
                offsetY = EditorGUILayout.FloatField(new GUIContent("Offset Y:"), offsetY);

            }

            if (GUILayout.Button("Import"))
            {
                AssetImportPostProcessing.SetUp(isSpritesheet, paddingX, paddingY, offsetX, offsetY);
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
                AssetImportPostProcessing.settingsEnabled = false;
            }

        }

        private static string ImportFile(bool overwriteExisting, string localPath, string sourcePath)
        {
            var sourceFile = new FileInfo(sourcePath);

            // Ignore invalid paths
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

        public static int spriteWidth = 32;
        public static float spritePaddingY = 7;
        public static float spritePaddingX = 8;
        public static float spriteoffsetY = 0;
        public static float spriteOffsetX = 0;
        public static TextureImporterType textureImportType = TextureImporterType.Sprite;
        public static int pixelsPerUnit = 32;
        public static FilterMode filterMode = FilterMode.Point;
        public static TextureImporterCompression compressionMode = TextureImporterCompression.Uncompressed;
        public static bool isSpriteSheet;

        internal static void SetUp(bool isSpritesheet, float paddingX, float paddingY, float offsetX, float offsetY)
        {
            AssetImportPostProcessing.isSpriteSheet = isSpritesheet;
            AssetImportPostProcessing.spritePaddingX = paddingX;
            AssetImportPostProcessing.spritePaddingY = paddingY;
            AssetImportPostProcessing.spriteoffsetY = offsetX;
            AssetImportPostProcessing.spriteOffsetX = offsetY;
        }

        void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            if (!settingsEnabled)
            {
                return;
            }

            var importer = assetImporter as TextureImporter;
            importer.textureType = textureImportType;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.filterMode = filterMode;
            importer.textureCompression = compressionMode;

            if (isSpriteSheet)
            {
                importer.spriteImportMode = SpriteImportMode.Multiple;

                SpriteMetaData[] spritesheet = new SpriteMetaData[4];

                for (int i = 0; i < 4; i++)
                {
                    SpriteMetaData sprite = new SpriteMetaData();


                    sprite.name = importer.name + $"s_{i.ToString("00")}";
                    
                    sprite.rect = new Rect(spriteOffsetX + i * (spriteWidth + spritePaddingX), spriteoffsetY+ spritePaddingY, 32, 32);

                    spritesheet[i] = sprite;
                }

                importer.spritesheet = spritesheet;

                Debug.Log("Imprt settings applied");
            }
           
        }



    }
}
#endif