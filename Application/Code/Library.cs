using Alihan.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnTimeChangedEvent : UnityEvent<NotifyChangedEventArg> { }

public class Library : Singleton<Library>
{
    [SerializeField] private TimeTracker m_TimeTracker;
    [SerializeField] private List<Book> m_BookList = new List<Book>();
    [SerializeField] private List<Book> m_BorrowBookRecordList = new List<Book>();
    [SerializeField] private OnTimeChangedEvent m_TimeChangedEvent;

    private Dictionary<int, Book> m_LibraryCache = new Dictionary<int, Book>();

    public delegate void AddBookDelegate(string title, string author, int copyCount, int isbn);
    public static AddBookDelegate OnAddBook;

    public static Action<int> OnBorrowBook;
    public static Action<int> OnReturnBook;
    private void Start()
    {
        OnAddBook += AddBook;
        OnBorrowBook += BorrowBook;
        OnReturnBook += ReturnBook;

        m_TimeTracker = new TimeTracker(UIController.Instance.TMP_Time);
        m_TimeTracker.InitializeTimeTracker();

        m_TimeTracker.DayChangedEvent += TimeTracker_DayChangedEvent;
    }

    private void TimeTracker_DayChangedEvent(object sender, NotifyChangedEventArg e)
    {

    }

    private void Update()
    {
        m_TimeTracker.UpdateTime();
    }
    private void OnDestroy()
    {
        OnAddBook -= AddBook;
        OnBorrowBook -= BorrowBook;
        OnReturnBook -= ReturnBook;
    }
    private void AddBook(string title, string author, int copyCount, int isbn)
    {
        if (!IsBookInList(isbn))
        {
            Book book = new Book(title, author, copyCount, isbn);
            m_BookList.Add(book);
            m_LibraryCache.Add(isbn, book);
        }
    }
    private void BorrowBook(int isbn)
    {
        if (m_LibraryCache.TryGetValue(isbn, out Book existingBook))
        {
            existingBook.CopyCount--;
            if (!m_BorrowBookRecordList.Contains(existingBook))
            {
                m_BorrowBookRecordList.Add(existingBook);
                Debug.Log("Kitap alýndý ve süre baþladý...");
            }

        }
    }
    private void ReturnBook(int isbn)
    {
        if (m_LibraryCache.TryGetValue(isbn, out Book existingBook))
        {
            existingBook.CopyCount++;
            m_BorrowBookRecordList.Remove(existingBook);
            Debug.Log("Kitap teslim edildi ve süre sýfýrlandý...");
        }
    }
    public bool IsBookInList(int isbn)
        => m_LibraryCache.ContainsKey(isbn);
    public Book GetBook(int isbn)
        => m_LibraryCache.TryGetValue(isbn, out Book existingBook) ? existingBook : null;

}
