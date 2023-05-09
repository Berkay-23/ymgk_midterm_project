using UnityEngine;
using DG.Tweening;

public class PinwheelCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            PlayerMovements playerMovement = collision.gameObject.GetComponent<PlayerMovements>();
            playerMovement.respawnPoint = new Vector3(transform.position.x, transform.position.y + 3, 0);

            UpdateScore(playerMovement);
            MoveToIcon();
        }
    }

    private void MoveToIcon()
    {
        Vector3 targetPosition = GameObject.Find("pwIcon").transform.position;
        transform.DOMove(targetPosition, Random.Range(0.5f, 1.5f)).SetEase(Ease.OutBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

    }

    private void UpdateScore(PlayerMovements playerMovement)
    {
        playerMovement.score++;
        TMPro.TextMeshProUGUI scoreText = GameObject.Find("pwCounter").GetComponent<TMPro.TextMeshProUGUI>();
        scoreText.text = playerMovement.score.ToString();
    }
}
