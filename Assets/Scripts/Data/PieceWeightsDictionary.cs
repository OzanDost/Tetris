using System;
using Enums;

namespace Data
{
    [Serializable]
    public class PieceWeightsDictionary : SerializableDictionary<PieceType, int>
    {
    }
}