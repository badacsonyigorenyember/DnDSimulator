using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OverWriteConfirm : MonoBehaviour
    {
        [SerializeField] private Button _yes;
        [SerializeField] private Button _no;

        private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        private void Start() {
            _yes.onClick.AddListener(() => _tcs.SetResult(true));
            _no.onClick.AddListener(() => _tcs.SetResult(false));
        }

        public Task<bool> GetResult() {
            return _tcs.Task;
        }
    }
}
