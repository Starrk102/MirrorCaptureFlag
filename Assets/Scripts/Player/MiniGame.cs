using Flag;
using Flag.FlagState;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class MiniGame : NetworkBehaviour
    {
        public Button btn;
        public RectTransform runnerImage;
        public RectTransform successZone;
        public RectTransform parentRect;
        public float runnerSpeed = 200f;
        public bool isPlaying = false;
        private float runnerPosition = 0f;
        private float gameTime;
        private Flag.Flag flag;

        public void StartMiniGame(Flag.Flag flag, float time)
        {
            if (isLocalPlayer)
            {
                this.flag = flag;
                gameTime = time;
                isPlaying = true;
            
                runnerImage.anchoredPosition = new Vector2(-parentRect.rect.width / 2, runnerImage.anchoredPosition.y);
                successZone.gameObject.SetActive(true);
                gameObject.SetActive(true);
                btn.onClick.AddListener(OnClickMiniGame);
            }
        }

        private void Update()
        {
            if (!isPlaying) return;
            
            float parentWidth = parentRect.rect.width;
            
            runnerPosition = Mathf.PingPong(Time.time * runnerSpeed, parentWidth);
            
            runnerImage.anchoredPosition = new Vector2(runnerPosition - (parentWidth / 2), runnerImage.anchoredPosition.y);
            
            gameTime -= Time.deltaTime;
            
            if (gameTime <= 0)
            {
                EndMiniGame();
            }
        }

        public void OnClickMiniGame()
        {
            if (isPlaying)
            {
                EndMiniGame();
            }
        }
        
        public void EndMiniGame()
        {
            if (isPlaying == true)
            {
                isPlaying = false;
                gameObject.SetActive(false);
        
                if (IsSuccessfulCapture())
                {
                    flag.TransitionToState(new CapturedState(flag));
                }
                else
                {
                    flag.ResetCapture();
                }
            
                btn.onClick.RemoveAllListeners();
            }
        }

        private bool IsSuccessfulCapture()
        {
            float runnerX = runnerImage.anchoredPosition.x;
            float successStart = successZone.anchoredPosition.x - (successZone.rect.width / 2);
            float successEnd = successZone.anchoredPosition.x + (successZone.rect.width / 2);

            return runnerX >= successStart && runnerX <= successEnd;
        }
    }
}
