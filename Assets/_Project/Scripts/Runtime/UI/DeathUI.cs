using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class DeathUI : MonoBehaviour
    {
        #region SERIALIZED_FIELDS

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI deathCountText;

        #endregion

        #region MONO

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region PUBLIC_METHODS


        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateDeathCount(int count)
        {
            if (deathCountText != null)
                deathCountText.text = count.ToString();
        }

        #endregion
    }
}

