using UnityEngine;

namespace _Project.Scripts
{
    public class RunInBackground : MonoBehaviour
    {
        private void Start()
        {
            Application.runInBackground = true;
        }
    }
}
