using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _controller;
    [SerializeField]
    private float _speed = 3.5f;
    private float _gravity = 9.8f;
    // Start is called before the first frame update
    [SerializeField]
    private GameObject _muzzleFlash;
    [SerializeField]
    private GameObject _hitMarkerPrefab;
    [SerializeField]
    private AudioSource _fireAudio;

    private int _currentAmmo;
    private int _maxAmmo = 50;
    private bool _isReloading = false;

    private UIManager _uiManager;

    public bool hasCoin = false;
    [SerializeField]
    private GameObject _weapon;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        //hide mouse cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _currentAmmo = _maxAmmo;

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
        CalculateMovement();
        UnhideMouse(); 
        if (Input.GetKeyDown(KeyCode.R) && _currentAmmo < _maxAmmo && _isReloading == false)
        {
            _isReloading = true;
            StartCoroutine(Reload());
        }
    }

    void Fire()
    {
        if (Input.GetMouseButton(0) && _currentAmmo > 0 && _weapon.activeInHierarchy)
        {
            Shoot();
        }
        else
        {
            _muzzleFlash.SetActive(false);
            _fireAudio.Stop();
        }
    }

    void Shoot()
    {
        _currentAmmo--;
        _uiManager.UpdateAmmo(_currentAmmo);
        _muzzleFlash.SetActive(true);
        if (_fireAudio.isPlaying == false)
        {
            _fireAudio.Play();
        }

        //cast ray from center point of main camera
        // Ray rayOrigin = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //what player can actually see
        RaycastHit hitInfo;
        if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity))
        {
            Debug.Log("Raycast Hit:" + hitInfo.transform.name);
            GameObject hitMarker = (GameObject)Instantiate(_hitMarkerPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(hitMarker, 1f);

            Destructables crate = hitInfo.transform.GetComponent<Destructables>();
            if(crate != null)
            {
                crate.DestroyCrate();
            }
        }
    }

    void UnhideMouse()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void CalculateMovement()
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        Vector3 velocity = direction * _speed;
        velocity.y -= _gravity;

        velocity = transform.transform.TransformDirection(velocity);
        _controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(1.5f);
        _currentAmmo = _maxAmmo;
        _uiManager.UpdateAmmo(_currentAmmo);
        _isReloading = false;
    }

    public void EnableWeapon()
    {
        _weapon.SetActive(true);
    }
}
