using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBookContainer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TitleTextMesh;
    [SerializeField] private TextMeshProUGUI m_AuthorTextMesh;
    [SerializeField] private TextMeshProUGUI m_CopyCountTextMesh;
    [SerializeField] private TextMeshProUGUI m_ISBNTextMesh;
    [SerializeField] private Button m_TakeButton;

    private int m_CopyCount;
    private int m_ISBN;
    public Button InteractButton => m_TakeButton;
    public int CopyCount
    {
        get => m_CopyCount;
        set => m_CopyCount = value;
    }
    public int GetISBN => m_ISBN;
    public void SetCopyCount(int copyCount)
    {
        m_CopyCountTextMesh.SetText($"CopyCount:{copyCount.ToString()}");
        m_CopyCount = copyCount;
    }
    public void SetISBN(int isbn)
    {
        m_ISBNTextMesh.SetText($"ISBN:{isbn.ToString()}");
        m_ISBN = isbn;
    }

    public void SetTitle(string title)
        => m_TitleTextMesh.SetText($"Title:{title}");
    public void SetAuthor(string author)
        => m_AuthorTextMesh.SetText($"Author:{author}");


}
