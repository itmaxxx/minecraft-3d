using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

	public bool isGrounded;
	public bool isSprinting;

	private new Transform camera;
	private World world;

	public float walkSpeed = 3f;
	public float spintSpeed = 6f;
	public float jumpForce = 5f;
	public float gravity = -9.8f;

	// Player "radius"
	public float playerWidth = 0.15f;

	private float horizontal;
	private float vertical;
	private float mouseHorizontal;
	private float mouseVertical;
	private Vector3 velocity;
	private float verticalMomentum = 0;
	private bool jumpRequest;

	public Transform highlightBlock;
	public Transform placeHighlightBlock;
	public float checkIncrement = 0.1f;
	public float reach = 8f;

	public byte selectedBlockID = 1;

	private AudioSource walkSource;
	private AudioSource digSource;

	private void Start()
	{
		camera = GameObject.Find("Main Camera").transform;
		world = GameObject.Find("World").GetComponent<World>();

		walkSource = GetComponents<AudioSource>()[0];
		digSource = GetComponents<AudioSource>()[1];

		Cursor.lockState = CursorLockMode.Locked;

		Application.targetFrameRate = 60;
	}

	private void FixedUpdate()
	{
		CalculateVelocity();

		if (jumpRequest)
			Jump();

		transform.Rotate(Vector3.up * mouseHorizontal);
		camera.Rotate(Vector3.right * -mouseVertical);
		transform.Translate(velocity, Space.World);
	}

	private void Update()
	{
		GetPlayerInputs();
		PlaceCursorBlocks();
	}

	private void Jump()
	{
		verticalMomentum = jumpForce;

		isGrounded = false;

		jumpRequest = false;
	}

	private void CalculateVelocity()
	{
		// Affect vertical momentum
		if (verticalMomentum > gravity)
			verticalMomentum += Time.fixedDeltaTime * gravity;

		// If we're sprinting, use the sprint multiplier
		if (isSprinting) {
			velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * spintSpeed;
			walkSource.pitch = 1.5f;
		}
		else {
			velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;
			walkSource.pitch = 1f;
		}

		// Walk sound
		if (velocity.x != 0 || velocity.z != 0) {
			if (!walkSource.isPlaying) {
				byte walkingBlockID = world.GetVoxel(new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z));

				Debug.Log(walkingBlockID);
				Debug.Log(transform.position.y - 0.5f);

				if (walkingBlockID != 0) {
					walkSource.clip = world.blockTypes[walkingBlockID].walkSound;
					walkSource.Play();
				}
			}
		}

		// Apply vertical momentum (fall/jump)
		velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

		if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
			velocity.z = 0;

		if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
			velocity.x = 0;

		if (velocity.y < 0)
			velocity.y = checkDownSpeed(velocity.y);
		else if (velocity.y > 0)
			velocity.y = checkUpSpeed(velocity.y);
	}

	private void GetPlayerInputs()
	{
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
		mouseHorizontal = Input.GetAxis("Mouse X");
		mouseVertical = Input.GetAxis("Mouse Y");

		if (Input.GetButtonDown("Sprint"))
			isSprinting = true;
		if (Input.GetButtonUp("Sprint"))
			isSprinting = false;

		if (isGrounded && Input.GetButtonDown("Jump"))
			jumpRequest = true;

		if (highlightBlock.gameObject.activeSelf)
		{
			// Break block
			if (Input.GetMouseButtonDown(0)) {
				int removedBlockID = world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);

				if (!digSource.isPlaying && removedBlockID != 0) {
					digSource.clip = world.blockTypes[removedBlockID].digSound;
					digSource.Play();
				}
			}

			// Place block
			if (Input.GetMouseButtonDown(1)) {
				world.GetChunkFromVector3(placeHighlightBlock.position).EditVoxel(placeHighlightBlock.position, selectedBlockID);
				
				if (!digSource.isPlaying) {
					digSource.clip = world.blockTypes[selectedBlockID].digSound;
					digSource.Play();
				}
			}
		}
	}

	private void PlaceCursorBlocks()
	{
		float step = checkIncrement;
		Vector3 lastPos = new Vector3();

		while (step < reach)
		{
			Vector3 pos = camera.position + (camera.forward * step);

			if (world.CheckForVoxel(pos))
			{
				highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
				placeHighlightBlock.position = lastPos;

				highlightBlock.gameObject.SetActive(true);
				placeHighlightBlock.gameObject.SetActive(true);

				return;
			}

			lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

			step += checkIncrement;
		}

		highlightBlock.gameObject.SetActive(false);
		placeHighlightBlock.gameObject.SetActive(false);
	}

	private float checkDownSpeed(float downSpeed)
	{
		if (
			world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
			world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
			world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
			world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))
			)
		{
			isGrounded = true;

			return 0;
		}
		else
		{
			isGrounded = false;

			return downSpeed;
		}
	}

	private float checkUpSpeed(float upSpeed)
	{
		if (
			world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1.8f + upSpeed, transform.position.z - playerWidth)) ||
			world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1.8f + upSpeed, transform.position.z - playerWidth)) ||
			world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1.8f + upSpeed, transform.position.z + playerWidth)) ||
			world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1.8f + upSpeed, transform.position.z + playerWidth))
			)
			return 0;
		else
			return upSpeed;
	}

	public bool front
	{
		get
		{
			// <!> Check for 2 more blocks
			if (
				world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
				world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
				)
				return true;
			else
				return false;
		}
	}

	public bool back
	{
		get
		{
			// <!> Check for 2 more blocks
			if (
				world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
				world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
				)
				return true;
			else
				return false;
		}
	}

	public bool left
	{
		get
		{
			// <!> Check for 2 more blocks
			if (
				world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
				world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
				)
				return true;
			else
				return false;
		}
	}

	public bool right
	{
		get
		{
			// <!> Check for 2 more blocks
			if (
				world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
				world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
				)
				return true;
			else
				return false;
		}
	}
}
