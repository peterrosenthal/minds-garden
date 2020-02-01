﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbScript : MonoBehaviour
{
    PlayerScript player;
    bool fading = false;
    bool following = false;
    bool moving = false;
    Vector3 rotPerSecond;
    Rigidbody rb;
    Vector3 velocity = Vector3.one;

    public bool soundOn;
    public AudioSource audioTrack;
    Image buttonLabel;
    public ParticleSystem particles;
    public ParticleSystem burst;
    public float rotationSpeed;
    public float y_offset;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerScript.S;
        rb = GetComponent<Rigidbody>();
        if (!soundOn)
        {
            audioTrack.volume = 0;
        }
        rotPerSecond = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * rotationSpeed;

        //Set Orb height to match terrain
        float y_pos = TerrainScript.S.GetTerrainHeight(transform.position.x, transform.position.y);
        y_pos += y_offset;
        transform.position = new Vector3(transform.position.x, y_pos, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        //Spin
        transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        if (following)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if(distance > 3)
            {
                transform.position = Vector3.SmoothDamp(transform.position, player.transform.position, ref velocity, 3f);

            }
            if (!moving)
            {
                moving = true;
                StartCoroutine(MoveOrb());
            }
        }
        if (transform.position.y < TerrainScript.S.GetTerrainHeight(transform.position.x, transform.position.z))
        {
            float y = TerrainScript.S.GetTerrainHeight(transform.position.x, transform.position.z);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        
    }

    private void OnMouseOver()
    {
        if(Vector3.Distance(this.transform.position, Camera.main.transform.position) < 10f)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                ToggleParticles();

                ToggleFollow();
            }
            //ShowButtonLabel();
        }
    }

    private void OnMouseExit()
    {
        //buttonLabel.gameObject.SetActive(false);
    }

    void ShowButtonLabel()
    {
        Vector3 labelPos = Camera.main.WorldToScreenPoint(this.transform.position);
        buttonLabel.transform.position = labelPos + (new Vector3(1,1,0) * 30);
        buttonLabel.gameObject.SetActive(true);
    }

    void ToggleParticles()
    {
        if (particles.isPlaying)
        {
            particles.Stop();
        }
        else
        {
            burst.Play();
            particles.Play();
        }
    }

    void ToggleFollow()
    {
        following = particles.isPlaying;
    }

    void ToggleVolume()
    {
        if (!fading)
        {
            fading = true;
            if (soundOn)
            {
                StartCoroutine(FadeOut());
                soundOn = false;
            }
            else
            {
                StartCoroutine(FadeIn());
                soundOn = true;
            }
        }
    }

    IEnumerator MoveOrb()
    {
        rb.AddForce(new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * 20);
        yield return new WaitForSeconds(1f);
        moving = false;
    }

    IEnumerator FadeIn()
    {
        audioTrack.volume = 0f;

        while (audioTrack.volume < 1f)
        {
            audioTrack.volume += Time.deltaTime;
            yield return null;
        }
        fading = false;
    }

    IEnumerator FadeOut()
    {
        audioTrack.volume = 1f;

        while (audioTrack.volume > 0f)
        {
            audioTrack.volume -= Time.deltaTime;
            yield return null;

        }
        fading = false;

    }
}
