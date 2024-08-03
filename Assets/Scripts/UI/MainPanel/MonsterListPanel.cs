using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileHandling;
using Loader;
using Models.Interfaces;
using Newtonsoft.Json;
using UI.MainPanel.ListElements;
using UnityEngine;


namespace UI.MainPanel
{
    public class MonsterListPanel : MonoBehaviour
    {
        [SerializeField] private Transform _listElementsContainer;

        [SerializeField] private GameObject _monstercreatureListElementPrefab;

        public async void ListMonsters() {
            string json = await File.ReadAllTextAsync(FileManager.MonsterManualPath);
            Dictionary<string, Monster> monsterDatas = JsonConvert.DeserializeObject<Dictionary<string, Monster>>(json);

            if (monsterDatas == null) return;
            
            foreach ((string monsterId, Monster monster) in monsterDatas) {
                AdventureLoader.Instance.PutInQueue(InstantiateMonster(monsterId, monster.Name));
            }
        }

        async Task InstantiateMonster(string monsterId, string monsterName) {
            MonsterListElement monsterListElement = Instantiate(_monstercreatureListElementPrefab, _listElementsContainer).GetComponent<MonsterListElement>();
            byte[] monsterImage = await File.ReadAllBytesAsync(FileManager.MonsterManualImagesPath + $"{monsterName}.png");
            
            monsterListElement.SetMonster(monsterName, monsterId, monsterImage);
        }
    }
}