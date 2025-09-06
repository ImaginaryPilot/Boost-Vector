using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 200f;
    [SerializeField] float mainThrust = 1f;
    [SerializeField] float levelloaddelay = 2f;

    [SerializeField] AudioClip mainengine;
    [SerializeField] AudioClip levelcomplete;
    [SerializeField] AudioClip deaths;

    [SerializeField] ParticleSystem engineparticles;
    [SerializeField] ParticleSystem levelparticles;
    [SerializeField] ParticleSystem deathparticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool collisionsDiabled = false;

    enum State {Alive,Dying,Transcending}
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondtoThrust();
            RespondtoRotate();
        }
        if (Debug.isDebugBuild)
        {
            Admin();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionsDiabled) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccess();
                break;
            default:
                StartDeath();
                break;
        }
    }

    void StartDeath()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deaths);
        deathparticles.Play();
        Invoke("Restart", levelloaddelay);
    }

    void StartSuccess()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelcomplete);
        levelparticles.Play();
        Invoke("LoadNextScene", levelloaddelay);
    }

    void Restart()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    void LoadNextScene()
    {
        int currentsceneindex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentsceneindex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void RespondtoRotate()
    {
        rigidBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }

    void RespondtoThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Thrusting();
        }
        else
        {
            audioSource.Stop();
            engineparticles.Stop();
        }
    }

    void Thrusting()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainengine);
        }
        engineparticles.Play();
    }

    void Admin()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKey(KeyCode.C))
        {
            collisionsDiabled = !collisionsDiabled;
        }
    }
}
