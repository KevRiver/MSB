using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    // Map Tile Block
    public GameObject blockGray_12;
    public GameObject blockGray_11;
    public GameObject blockBrown_12;
    public GameObject blockBrown_11;

    // Map Object
    public GameObject jumpPad;
    public GameObject sampleEnemy;


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
        for (int y = 0; y < mapStruct.mapHeight; y++)
        {
            for (int x = 0; x < mapStruct.mapWidth; x++)
            {
               
                switch(mapStruct.mapArray[x, y])
                {
                    case '1':
                        Instantiate(blockGray_12, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.5f - y, 0), Quaternion.identity);
                        break;
                    case '2':
                        Instantiate(blockGray_11, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.5f - y, 0), Quaternion.identity);
                        break;
                    case '3':
                        Instantiate(blockBrown_12, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.5f - y, 0), Quaternion.identity);
                        break;
                    case '4':
                        Instantiate(blockBrown_11, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.5f - y, 0), Quaternion.identity);
                        break;
                }

                switch (mapStruct.objectArray[x, y])
                {
                    case '2':
                        Instantiate(jumpPad, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.85f - y, 0), Quaternion.identity);
                        break;
                    case '3':
                        Instantiate(sampleEnemy, new Vector3(x - (mapStruct.mapWidth / 2), mapStruct.mapHeight - 5.5f - y, 0), Quaternion.identity);
                        break;
                }

            }
        }
    }
}
