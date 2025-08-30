using UnityEngine;

public class DiffViewer : MonoBehaviour
{
    // 1 - 5까지의 난이도가 존재하며, 별 오브젝트가 몇개나 켜졋는지로 표현
    [SerializeField] private GameObject[] stars; // 0: 1 star, 1: 2 stars, ..., 4: 5 stars
    [SerializeField] private int difficulty; // 1 ~ 5
    
    public void SetDifficulty(int diff)
    {
        difficulty = Mathf.Clamp(diff, 1, 5);
        UpdateStars();
    }

    private void UpdateStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            // 별 색깔을 칠하기
            stars[i].GetComponent<SpriteRenderer>().color = (i < difficulty) ? Color.black : Color.gray;
        }
    }
}
