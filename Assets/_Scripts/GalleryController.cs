﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GalleryController : MonoBehaviour
{
    public static string screenshotsPath;
    public GameObject galleryPrefab;
    public List<string> allScreenshotPaths;

    void Start()
    {
        allScreenshotPaths = new List<string>();
        screenshotsPath = Path.Combine(Application.persistentDataPath, "Screenshots");
        LoadScreenshots();
    }

    void LoadScreenshots()
    {
        if (Directory.Exists(screenshotsPath))
        {
            foreach (string f in Directory.GetFiles(screenshotsPath))
            {
                allScreenshotPaths.Add(f);
            }


        }

        int screenshotCount = allScreenshotPaths.Count;
        int imageIndex = 0;
        int galleryIndex = 0;
        if(screenshotCount > 0)
        {
            foreach (Transform galPanel in transform)
            {
                GalleryPanel panel = galPanel.GetComponent<GalleryPanel>();
                foreach (Image image in panel.images)
                {
                    if (imageIndex < screenshotCount)
                    {
                        byte[] bytes = File.ReadAllBytes(allScreenshotPaths[imageIndex]);
                        Texture2D texture = new Texture2D(1, 1);
                        texture.LoadImage(bytes);
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        image.sprite = sprite;
                        imageIndex++;
                    }
                    else
                    {
                        image.enabled = false;
                        panel.backgrounds[imageIndex].enabled = false;
                        imageIndex++;
                    }


                }

                galleryIndex++;
                if (imageIndex >= screenshotCount)
                {
                    panel.nextButton.SetActive(false);
                }
            }

            //TODO: Instantiate new panels for more images
            while(imageIndex < screenshotCount)
            {
                Vector3 newPanelPos = transform.GetChild(0).GetComponent<RectTransform>().localPosition;
                newPanelPos.x += 1080 * galleryIndex;
                GameObject galPanel = Instantiate(galleryPrefab, this.transform);
                galPanel.GetComponent<RectTransform>().localPosition = newPanelPos;

                GalleryPanel panel = galPanel.GetComponent<GalleryPanel>();
                foreach (Image image in panel.images)
                {
                    if (imageIndex < screenshotCount)
                    {
                        byte[] bytes = File.ReadAllBytes(allScreenshotPaths[imageIndex]);
                        Texture2D texture = new Texture2D(1, 1);
                        texture.LoadImage(bytes);
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        image.sprite = sprite;
                        imageIndex++;
                    }
                    else
                    {
                        image.enabled = false;
                        panel.backgrounds[imageIndex % 8].enabled = false;
                        imageIndex++;
                    }


                }

                galleryIndex++;
                if (imageIndex >= screenshotCount)
                {
                    panel.nextButton.SetActive(false);
                }
            }
        }
    }
}
