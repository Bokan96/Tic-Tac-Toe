using System.IO;
using UnityEngine;
using TicTacToe.Core;
using TicTacToe.Data;
using TicTacToe.Utilities;

namespace TicTacToe.Managers
{
    public class StatisticsManager : SingletonMonoBehaviour<StatisticsManager>
    {
        private StatisticsData _data;
        private string SavePath => Path.Combine(Application.persistentDataPath, "stats.json");

        public StatisticsData Data => _data;

        protected override void Awake()
        {
            base.Awake();
            Load();
        }

        private void OnEnable()  => GameEvents.OnGameEnded += RecordResult;
        private void OnDisable() => GameEvents.OnGameEnded -= RecordResult;


        private void RecordResult(GameResult result)
        {
            _data.totalGames++;
            _data.totalDuration += result.Duration;

            if      (result.IsDraw)                    _data.draws++;
            else if (result.Winner == PlayerMark.X)    _data.player1Wins++;
            else                                       _data.player2Wins++;

            Save();
        }

        public void ResetStats()
        {
            _data = new StatisticsData();
            Save();
        }
        private void Load()
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                _data = JsonUtility.FromJson<StatisticsData>(json);
            }
            else
            {
                _data = new StatisticsData();
            }
        }

        private void Save()
        {
            string json = JsonUtility.ToJson(_data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
        }
    }
}
