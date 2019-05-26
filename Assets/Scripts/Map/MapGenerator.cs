using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    // Map Tile Block
    public GameObject blockGray_11;
    public GameObject blockBrown_11;

    // Map Object
    public GameObject jumpPad;
    public GameObject sampleEnemy;
    public GameObject flag;

    // Camera
    public GameObject mainCamera;


    // 맵 속성 구조체
    public struct MapStruct
    {
        public int mapHeight;
        public int mapWidth;
        public char[,] mapArray;
        public char[,] objectArray;

        public MapStruct(int height, int width)
        {
            mapHeight = height;
            mapWidth = width;
            mapArray = new char[width, height];
            objectArray = new char[width, height];

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MapStruct map1 = new MapStruct();
        map1 = loadFile(map1);
        printMap(map1);
        // 카메라에 맵 넓이 전달
        mainCamera = GameObject.Find("Main Camera");
        Debug.Log(mainCamera);
        mainCamera.GetComponent<followCamera>().mapHeight = map1.mapHeight;
        mainCamera.GetComponent<followCamera>().mapWidth = map1.mapWidth;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 텍스트 파일 로드 및 구조체 초기화
    MapStruct loadFile(MapStruct mapStruct)
    {
        // 텍스트 파일로 만든 맵 파일 로드
        string loadedFile = File.ReadAllText(@"Assets/Scripts/MapText.txt");
        string loadedObjectFile = File.ReadAllText(@"Assets/Scripts/MapObjectText.txt");

        // 맵의 가로와 세로 측정
        int height = 1;
        int width = loadedFile.Length;

        for (int x = 0; x < loadedFile.Length; x++)
        {
            if (loadedFile[x] == '\n')
            {
                height++;

            }
        }
        width = (width + 1 - height) / height;

        // 구조체 초기화
        mapStruct = new MapStruct(height, width);

        // 구조체 배열에 맵 나눠서 집어넣기
        int x1 = 0;
        int y1 = 0;
        for (int x = 0; x < loadedFile.Length; x++)
        {
            if (loadedFile[x] != '\n')
            {
                mapStruct.mapArray[x1, y1] = loadedFile[x];
                mapStruct.objectArray[x1, y1] = loadedObjectFile[x];
                x1++;
            }
            else
            {
                y1++;
                x1 = 0;
            }
        }

        // 구조체 리턴
        return mapStruct;
    }

    // 맵 오브젝트 출력
    void printMap(MapStruct mapStruct)
    {
        // 맵 파일을 기반으로 오브젝트 생성
        int putBlockID = 0;
        int putObjectID = 0;
        for (int y = 0; y < mapStruct.mapHeight; y++)
        {
            for (int x = 0; x < mapStruct.mapWidth; x++)
            {
                GameObject generatedObject;
                switch(mapStruct.mapArray[x, y])
                {
                    case '2':
                        generatedObject = Instantiate(blockGray_11, new Vector3(x, -y, 0), Quaternion.identity);
                        generatedObject.GetComponent<BlockData>().objectType = 0;
                        generatedObject.GetComponent<BlockData>().blockID = putBlockID;
                        putBlockID++;
                        //Debug.Log(generatedObject);
                        //Debug.Log(generatedObject.GetComponent<BlockData>().blockID);
                        break;
                    case '4':
                        generatedObject = Instantiate(blockBrown_11, new Vector3(x, -y, 0), Quaternion.identity);
                        generatedObject.GetComponent<BlockData>().objectType = 1;
                        generatedObject.GetComponent<BlockData>().blockID = putBlockID;
                        //Debug.Log(generatedObject);
                        //Debug.Log(generatedObject.GetComponent<BlockData>().blockID);
                        putBlockID++;
                        break;
                        /*
                        case '2':
                            block = Instantiate(blockGray_11, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.5f - y, 0), Quaternion.identity);
                            block.GetComponent<BlockData>().blockID = putBlockID;
                            putBlockID++;
                            break;
                        case '4':
                            block = Instantiate(blockBrown_11, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.5f - y, 0), Quaternion.identity);
                            block.GetComponent<BlockData>().blockID = putBlockID;
                            putBlockID++;
                            break;
                            */
                }

                switch (mapStruct.objectArray[x, y])
                {
                    /*
                    case '2':
                        block = Instantiate(jumpPad, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.85f - y, 0), Quaternion.identity);
                        block.GetComponent<BlockData>().objectID = putObjectID;
                        putObjectID++;
                        break;
                    case '3':
                        Instantiate(sampleEnemy, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.5f - y, 0), Quaternion.identity);
                        break;
                        */

                    case '2':
                        generatedObject = Instantiate(jumpPad, new Vector3(x,- 0.35f - y, 0), Quaternion.identity);
                        generatedObject.GetComponent<BlockData>().objectType = 10;
                        generatedObject.GetComponent<BlockData>().objectID = putObjectID;
                        putObjectID++;
                        break;
                    case '3':
                        generatedObject = Instantiate(sampleEnemy, new Vector3(x, -y, 0), Quaternion.identity);
                        generatedObject.GetComponent<BlockData>().objectType = 12;
                        break;
                    case '4':
                        generatedObject = Instantiate(flag, new Vector3(x, 0.55f -y, 0), Quaternion.identity);
                        generatedObject.GetComponent<BlockData>().objectType = 11;
                        break;
                }

            }
        }
    }
}
