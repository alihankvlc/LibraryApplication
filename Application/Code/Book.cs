using UnityEngine;

[System.Serializable]
public class Book
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Author { get; private set; }
    [field: SerializeField] public int ISBN { get; private set; }
    [field: SerializeField] public int CopyCount { get; set; }
    public Book(string title, string author, int isbn, int copyCount)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        CopyCount = copyCount;
    }
}
