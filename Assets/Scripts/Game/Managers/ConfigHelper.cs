using System.Linq;
using Data;
using Enums;
using UnityEngine;

namespace Game.Managers
{
    public static class ConfigHelper
    {
        public static GameConfig Config => _config;
        private static GameConfig _config;

        public static void Initialize()
        {
            LoadConfig();
        }

        private static void LoadConfig()
        {
            _config = Resources.Load<GameConfig>("GameConfig");
        }

        public static PieceType GetRandomPieceType()
        {
            int weightSum = _config.PieceWeightsDictionary.Values.Sum(weight => weight);

            if (weightSum == 0)
            {
                int randomTargetIndex = Random.Range(0, _config.PieceWeightsDictionary.Count);
                return _config.PieceWeightsDictionary.Keys.ElementAt(randomTargetIndex);
            }

            int randomWeight = Random.Range(0, weightSum);
            int currentWeightSum = 0;

            foreach (var pieceWeight in _config.PieceWeightsDictionary)
            {
                currentWeightSum += pieceWeight.Value;
                if (currentWeightSum > randomWeight)
                    return pieceWeight.Key;
            }

            return PieceType.BoxShape;
        }
    }
}