using UnityEngine;

public class PongFrontend : MonoBehaviour {
	[SerializeField]
	private new KMAudio audio;
	private int audioTimer = 0;
	private KMAudio.KMAudioRef audioRef = null;

	public float padding = 0.1f;
	public Vector2 size;
	[SerializeField]
	private GameObject ball;
	[SerializeField]
	private GameObject paddle1;
	[SerializeField]
	private GameObject paddle2;
	[SerializeField]
	private KMSelectable[] buttons;
	public int heldButton = -1;

	private PongBackend backend;

	// Use this for initialization
	public void Init(PongBackend backend) {
		this.backend = backend;
		this.size = new Vector2(1.0f - 2*padding, 1.0f);
		Vector3 UnitScale = 0.2f * new Vector3(1/this.transform.localScale.x, 1, 1/this.transform.localScale.z);
		this.ball.transform.localScale = UnitScale * this.backend.BallSize;
		Vector3 PaddleScale = new Vector3(UnitScale.x * padding, 1, UnitScale.z * this.backend.PaddleSize);
		this.paddle1.transform.localScale = PaddleScale;
		this.paddle2.transform.localScale = PaddleScale;

		buttons[0].OnInteract += delegate() {return ButtonPressed(0);};
		buttons[1].OnInteract += delegate() {return ButtonPressed(1);};
		buttons[2].OnInteract += delegate() {return ButtonPressed(2);};
		buttons[3].OnInteract += delegate() {return ButtonPressed(3);};
			
		for (int i = 0; i < 4; i++) {
			buttons[i].OnInteractEnded += ButtonReleased;
		}
	}
	
	//Update positions of game objects
	void Update() {
		if (this.audioRef != null) {
			this.audioTimer--;
			if (this.audioTimer <= 0) {
				StopSound();
			}
		}

		if (this.backend.playing) {
			this.ball.transform.localPosition = new Vector3(this.padding + this.backend.BallPos.x - 0.5f, 0, this.backend.BallPos.y - 0.5f);
		} else {
			this.ball.transform.localPosition = new Vector3(0, -0.1f, 0);
		}
		
		this.paddle1.transform.localPosition = new Vector3((this.padding - 1.0f) / 2f + 0.02f, 0, this.backend.Paddle1Y - 0.5f);
		this.paddle2.transform.localPosition = new Vector3((1.0f - this.padding) / 2f - 0.02f, 0, this.backend.Paddle2Y - 0.5f);
	}

	private bool ButtonPressed(int buttonId) {
		this.heldButton = buttonId;
		return false;
	}

	private void ButtonReleased() {
		this.heldButton = -1;
	}

	public void PlayBeep() {
		this.audio.PlaySoundAtTransform("4391__noisecollector__pongblipf-5", this.ball.transform);
	}

	public void PlayGameStart() {
		StopSound();
		this.audioRef = this.audio.PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.NeedyWarning, this.transform);
		this.audioTimer = 300;
	}

	private void StopSound() {
		if (this.audioRef != null) {
			this.audioRef.StopSound();
			this.audioRef = null;
		}
	}
}
