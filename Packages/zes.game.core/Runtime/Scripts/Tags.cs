using UnityEngine;

namespace Zes
{
    public class Tags : MonoBehaviour
    {
        public TagData[] tags;

        public GameObject GetTag(string name)
        {
            foreach (var tag in tags)
            {
                if (tag.tagName == name)
                {
                    return tag.tagGo;
                }
            }
            return null;
        }
    }
}
