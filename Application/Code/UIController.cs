using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alihan.UI
{
    public class UIController : Singleton<UIController> 
    {
        [Header("InputField Variables")]
        [SerializeField] private TMP_InputField m_TitleInput;
        [SerializeField] private TMP_InputField m_AuthorInput;
        [SerializeField] private TMP_InputField m_CopyCountText;
        [SerializeField] private TMP_InputField m_ISBNInput;
        [SerializeField] private TMP_InputField m_SearchInput;

        [Header("Place Holder Variables")]
        [SerializeField] private Transform m_UIBookPlaceHolder;
        [SerializeField] private Transform m_UIBorrowBookPlaceHolder;

        [Header("Instantiate UI Element Variables")]
        [SerializeField] private GameObject m_UIBookObject;
        [SerializeField] private GameObject m_UIBorrowBookObject;

        [Header("Interactable UI Element Variables")]
        [SerializeField] private Button m_AddBookButton;
        [SerializeField] private Button m_SearchButton;
        [SerializeField] private Button m_RandomBookButton;

        [Header("Text Variables")]
        [SerializeField] private TextMeshProUGUI m_SystemMessageTextMesh;
        [SerializeField] private TextMeshProUGUI m_TimeTextMesh;

        [Header("Other")]
        [SerializeField] private RandomBook[] m_RandomBookArray;

        private List<GameObject> m_UIBorrewedBookList = new List<GameObject>();
        private Dictionary<int, GameObject> m_UIBookCache = new Dictionary<int, GameObject>();
        public TextMeshProUGUI TMP_Time
        {
            get => m_TimeTextMesh;
            set => m_TimeTextMesh = value;
        }
        private void Start()
        {
            m_AddBookButton.onClick.AddListener(OnAddBookButtonClicked);
            m_SearchButton.onClick.AddListener(OnSearchButtonClicked);
            m_RandomBookButton.onClick.AddListener(OnAddRandomBookButtonClicked);

        }

        #region Button Click Event Handlers
        private void OnAddBookButtonClicked()
        {
            if (InputsEmpty())
            {
                StartCoroutine(ShowSystemMessage(2f, "Issue adding the book",
                    SystemMessageType.Unsuccessful));

                return;
            }

            string title = m_TitleInput.text;
            string author = m_AuthorInput.text;
            string copyCount = m_CopyCountText.text;
            string isbn = m_ISBNInput.text;

            if (!Library.Instance.IsBookInList(ParseInputToInt(isbn)))
            {
                Library.OnAddBook?.Invoke(title, author, ParseInputToInt(copyCount), ParseInputToInt(isbn));
                UIAddedBook(title, author, ParseInputToInt(copyCount), ParseInputToInt(isbn));
                StartCoroutine(ShowSystemMessage(2f, "Book successfully added..."));
            }
            else
            {
                StartCoroutine(ShowSystemMessage(2f, "The book already exists in the list.",
                    SystemMessageType.Unsuccessful));
            }
        }
        private void OnAddRandomBookButtonClicked()
        {
            int randomIndex = UnityEngine.Random.Range(0, m_RandomBookArray.Length);

            m_TitleInput.text = m_RandomBookArray[randomIndex].Title;
            m_AuthorInput.text = m_RandomBookArray[randomIndex].Author;
            m_CopyCountText.text = m_RandomBookArray[randomIndex].CopyCount.ToString();
            m_ISBNInput.text = m_RandomBookArray[randomIndex].ISBN.ToString();
        }
        private void OnSearchButtonClicked()
        {
            string searchInputText = m_SearchInput.text.ToLower();
            foreach (Transform child in m_UIBookPlaceHolder)
            {
                string childName = child.name.ToLower();
                child.gameObject.SetActive(childName.Contains(searchInputText));
            }
        }
        #endregion
        #region UI Display Functions
        private void UIAddedBook(string title, string author, int copyCount, int isbn)
        {
            GameObject spawnBookObject = Instantiate(m_UIBookObject);
            spawnBookObject.transform.SetParent(m_UIBookPlaceHolder, false);
            spawnBookObject.name = title;

            UIBookContainer container = spawnBookObject.GetComponent<UIBookContainer>();
            if (container != null)
            {
                container.SetTitle(title);
                container.SetAuthor(author);
                container.SetCopyCount(copyCount);
                container.SetISBN(isbn);

                container.InteractButton.onClick.AddListener(() => UIBorrowedBook(isbn));
            }

            m_UIBookCache.Add(isbn, spawnBookObject);
        }

        private void UIBorrewedAddBook(int isbn)
        {
            GameObject spawnBorrewedBook = Instantiate(m_UIBorrowBookObject);
            spawnBorrewedBook.transform.SetParent(m_UIBorrowBookPlaceHolder, false);

            UIBookContainer container = spawnBorrewedBook.GetComponent<UIBookContainer>();
            Book book = Library.Instance.GetBook(isbn);
            if (container != null)
            {
                container.SetTitle(book.Title);
                container.SetAuthor(book.Author);
                container.SetISBN(isbn);

                container.InteractButton.onClick.AddListener(() => UIReturnBook(isbn));
                m_UIBorrewedBookList.Add(container.gameObject);
            }
        }

        private void UIReturnBook(int isbn)
        {
            Library.OnReturnBook?.Invoke(isbn);

            if (m_UIBookCache.TryGetValue(isbn, out GameObject existingBookObj))
            {
                UIBookContainer container = existingBookObj.GetComponent<UIBookContainer>();
                GameObject borrowedBookObj = m_UIBorrewedBookList.First(r => r.GetComponent<UIBookContainer>().GetISBN == isbn);
                m_UIBorrewedBookList.Remove(borrowedBookObj);
                Destroy(borrowedBookObj);
                SetBookCopyCount(container, ValueState.Increase);
            }
        }

        private void UIBorrowedBook(int isbn)
        {
            Library.OnBorrowBook?.Invoke(isbn);
            if (m_UIBookCache.TryGetValue(isbn, out GameObject existingBookObj))
            {
                UIBookContainer container = existingBookObj.GetComponent<UIBookContainer>();
                SetBookCopyCount(container);
                UIBorrewedAddBook(isbn);
            }
        }

        private void SetBookCopyCount(UIBookContainer container, ValueState valState = ValueState.Decrease)
        {
            int newValue = valState == ValueState.Decrease ? -1 : +1;
            container?.SetCopyCount(container.CopyCount + newValue);
        }
        #endregion
        #region Utility Functions
        private bool InputsEmpty()
        {
            return string.IsNullOrWhiteSpace(m_TitleInput.text) ||
                     string.IsNullOrWhiteSpace(m_AuthorInput.text) ||
                     string.IsNullOrWhiteSpace(m_CopyCountText.text) ||
                     string.IsNullOrWhiteSpace(m_ISBNInput.text);
        }

        private IEnumerator ShowSystemMessage(float duration, string message, SystemMessageType messageType = SystemMessageType.Succefly)
        {
            WaitForSeconds wait = new WaitForSeconds(duration);

            ClearAllInputs();
            m_SystemMessageTextMesh.color = messageType == SystemMessageType.Succefly ? Color.yellow : Color.red;
            m_SystemMessageTextMesh.SetText(message);
            m_SystemMessageTextMesh.gameObject.SetActive(true);

            yield return wait;
            m_SystemMessageTextMesh.gameObject.SetActive(false);
        }

        private void ClearAllInputs()
        {
            m_TitleInput.text = string.Empty;
            m_AuthorInput.text = string.Empty;
            m_CopyCountText.text = string.Empty;
            m_ISBNInput.text = string.Empty;
        }

        private int ParseInputToInt(string param)
        {
            int returnToValue = 0;

            if (int.TryParse(param, out int value))
                returnToValue = value;

            return returnToValue;
        }
        #endregion
    }
}

