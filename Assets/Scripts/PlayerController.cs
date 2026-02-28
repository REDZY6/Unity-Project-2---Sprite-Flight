using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float thrustForce = 1.0f;
    public float maxSpeed = 5.0f;

    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;
    private int savedHighScore;

    public UIDocument uiDocument;
    private Label scoreText;
    private Label highScoreText;
    private Button restartButton;

    public GameObject boosterFlame;
    public GameObject explosionEffect;
    public GameObject borderParent;

    Rigidbody2D rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // .rootVisualElement gives access to the top-level container of the UI layout
        // .Q<Label>("ScoreLabel") uses Unity's Query system to find the first element of type "Label" with the name ScoreLabel
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");

        // Load High Score. If it does not exist default to 0.
        highScoreText.style.display = DisplayStyle.None;
        savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + savedHighScore;

        // Setting display style to None removes it from the layout
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;
    }

    // Update is called once per frame
    void Update()
    {
       
        UpdateScore();
        MovePlayer();

        void MovePlayer() {
            if (Mouse.current.leftButton.isPressed)
            {
                // Calculate mouse direction
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
                Vector2 direction = (mousePos - transform.position).normalized;

                // Move player in direction of mouse
                transform.up = direction;
                rb.AddForce(direction * thrustForce);

                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                boosterFlame.SetActive(true);
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                boosterFlame.SetActive(false);
            }
        }

        void UpdateScore()
        {
            elapsedTime += Time.deltaTime;
            score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
            scoreText.text = "Score: " + score;
        }
      
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.name != "Player" && !gameObject.CompareTag("Player"))
        {
            return;
        };

        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Wall"))
        {
            int finalScore = Mathf.FloorToInt(score);
            borderParent.SetActive(false);
            
            if (finalScore > savedHighScore)
            {
                //Update the variable and save it to the disk
                savedHighScore = finalScore;
                PlayerPrefs.SetInt("HighScore", savedHighScore);
                PlayerPrefs.Save();

                highScoreText.text = "New Best: " + savedHighScore;
                highScoreText.style.color = new StyleColor(Color.yellow);
                highScoreText.style.scale = new StyleScale(new Vector2(1.2f, 1.2f)); 
            }
            Destroy(gameObject);
            // Instantiate() to create a copy of the prefab
            Instantiate(explosionEffect, transform.position, transform.rotation);
            // Flex is a system used in UI Toolkit where elements are laid out in a flexble row or column
            // In this case we are displaying the element using standard flexible layout behaviour.
            restartButton.style.display = DisplayStyle.Flex;
            highScoreText.style.display = DisplayStyle.Flex;
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
