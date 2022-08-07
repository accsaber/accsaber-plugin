using System.Threading.Tasks;
using AccSaber.Downloaders;
using AccSaber.Models;
using UnityEngine;

namespace AccSaber.Interfaces
{
    public interface ILeaderboardSource
    {
        public string HoverHint { get; }
        public Sprite Icon { get; }

        public bool Scrollable { get; }
    }
}