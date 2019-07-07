using UnityEngine;

public class PongBackend : MonoBehaviour {
	[SerializeField]
	private KMNeedyModule module;
	[SerializeField]
	private PongFrontend frontend;
	public float BallSize = 0.075f;
	public float PaddleSize = 0.1f;
	public Vector2 BallPos = new Vector2(0.5f, 0.5f);
	public Vector2 BallVel = new Vector2();
	private Vector2 size;
	public bool playing = false;

	public float Paddle1Y = 0.5f;
	public float Paddle2Y = 0.5f;
	private float PaddleSpeed = 0.3f;

	private int bouncesRequired = 0;

	private void Awake() {
		frontend.Init(this);
		this.size = frontend.size;
		this.module.OnNeedyActivation += OnNeedyActivation;
		this.module.OnNeedyDeactivation += OnNeedyDeactivation;
		this.module.OnTimerExpired += OnTimerExpired;
		this.module.WarnAtFiveSeconds = false;
		this.Paddle1Y = 0.5f;
		this.Paddle2Y = 0.5f;
	}

	// Update is called once per frame
	void Update() {
		HandleInputs();
		if (playing) {
			PongTick();
			this.module.SetNeedyTimeRemaining(bouncesRequired + 0.1f);
		} else {
			if (this.module.GetNeedyTimeRemaining() < 0.99f && this.module.GetNeedyTimeRemaining() > -1f) Play();
		}
	}

	private void PongRestart() {
		this.BallPos = this.size / 2.0f;
		this.BallVel = new Vector2(Mathf.PI + Random.Range(-1f, 1f), Mathf.Sqrt(2) + Random.Range(-1f, 1f)) / 40f;
	}

	private void PongTick() {
		this.BallPos += BallVel * Time.deltaTime;
		if (BallPos.y - BallSize < 0 || BallPos.y + BallSize > size.y) {
			BallVel = new Vector2(BallVel.x, -BallVel.y);
		}
		if (BallPos.x - BallSize < 0) {
			if (Mathf.Abs(BallPos.y - Paddle1Y) < PaddleSize + BallSize) {
				HandleBounce();
			} else {
				Resolve();
			}
		}
		if (BallPos.x + BallSize > size.x) {
			if (Mathf.Abs(BallPos.y - Paddle2Y) < PaddleSize + BallSize) {
				HandleBounce();
			} else {
				Resolve();
			}
		}
		this.BallPos = new Vector2(Mathf.Clamp(this.BallPos.x, BallSize, size.x - BallSize), Mathf.Clamp(this.BallPos.y, BallSize, size.y - BallSize));
	}

	private void HandleInputs() {
		int input = this.frontend.heldButton;
		if (input > -1) {
			switch (input) {
				case 0: {
					this.Paddle1Y += this.PaddleSpeed * Time.deltaTime;
					break;
				}
				case 1: {
					this.Paddle1Y -= this.PaddleSpeed * Time.deltaTime;
					break;
				}
				case 2: {
					this.Paddle2Y += this.PaddleSpeed * Time.deltaTime;
					break;
				}
				case 3: {
					this.Paddle2Y -= this.PaddleSpeed * Time.deltaTime;
					break;
				}
			}
			this.Paddle1Y = Mathf.Clamp(this.Paddle1Y, this.PaddleSize, size.y - this.PaddleSize);
			this.Paddle2Y = Mathf.Clamp(this.Paddle2Y, this.PaddleSize, size.y - this.PaddleSize);
		}
	}

	private void Resolve() {
		EndGame(this.bouncesRequired == 0);
	}

	private void HandleBounce() {
		BallVel = new Vector2(-BallVel.x, BallVel.y);
		this.bouncesRequired--;
		if (this.bouncesRequired < 0) {
			EndGame(false);
		} else {
			this.frontend.PlayBeep();
		}
	}

	private void Play() {
		this.playing = true;
		this.bouncesRequired = Random.Range(1, 4);
		this.frontend.PlayGameStart();
		PongRestart();
	}

	private void EndGame(bool win) {
		if (win) {
			this.module.HandlePass();
		} else {
			this.module.HandleStrike();
		}
		this.bouncesRequired = 0;
		this.playing = false;
	}

    protected void OnNeedyActivation() {
		this.module.SetNeedyTimeRemaining(10.1f);
    }

    protected void OnNeedyDeactivation() {
		this.playing = false;
	}

	protected void OnTimerExpired() {

	}
}
