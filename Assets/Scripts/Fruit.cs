using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Slot
{
    Peach = 5,
    Plum = 10,
    Pomegranate = 60,
    Lemon = 30,
    Watermelon = 100,
    Apple = 2,
    Orange = 20,
    Default
}

public class Fruit : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    public float destroyTime = 0.5f;
    private Image fruitIcon;
    [NonSerialized] public Slot slot = Slot.Default;
    private Coroutine animateCoroutine;
    private Image fireImage;
    private Sprite fireV;
    private Sprite fireH;
    private float animTime = 0.75f;
    
    private void Awake()
    {
        fruitIcon = GetComponent<Image>();
        if(fruitIcon == null)
            Debug.LogError("Image component is not founded");

        fireImage = transform.Find("Fire").GetComponent<Image>();
        if(fireImage == null)
            Debug.LogError("Image fire component is not founded");
    }

    private void Start()
    {
        fireImage.gameObject.SetActive(false);
        fireH = GetSprite("Fire", "FireH");
        fireV = GetSprite("Fire", "FireV");
        SetSlot(Slot.Default);
    }

    public void SetRandomSlot()
    {
        SetSlot(GetRandomSlot());
    }

    private void SetSlot(Slot slot)
    {
        if (animateCoroutine != null)
        {
            StopCoroutine(animateCoroutine);
        }
        AnimateFruit(false);
        this.slot = slot;
        Sprite sprite = GetSprite("Slot", slot.ToString());
        
        if (sprite != null)
        {
            fruitIcon.sprite = sprite;
        }
    }
    
    private Slot GetRandomSlot()
    {
        Slot[] values = (Slot[])System.Enum.GetValues(typeof(Slot));
    
        // Создаем список без значения Slot.Default
        List<Slot> validValues = new List<Slot>(values);
        validValues.Remove(Slot.Default);

        // Выбираем случайное значение из списка
        Slot randomValue = validValues[UnityEngine.Random.Range(0, validValues.Count)];
        
        return randomValue;
    }
    
    private Sprite GetSprite(string folder, string fileName)
    {
        // Загрузка спрайта из папки Resources/Slot
        Sprite sprite = Resources.Load<Sprite>($"{folder}/{fileName}");
        
        // Проверка, был ли успешно загружен спрайт
        if (sprite == null)
        {
            Debug.LogError($"Sprite with name '{fileName}' not found in {folder} folder.");
        }
        
        return sprite;
    }

    public void Match(bool isVertical)
    {
        GameObject effectObj = Instantiate(effect, transform.position, Quaternion.identity);
        Destroy(effectObj, destroyTime);
        
        //fireImage.gameObject.SetActive(true);
        
        //if (isVertical)
            //fireImage.sprite = fireV;
        //else
            //fireImage.sprite = fireH;
        
        //fireImage.SetNativeSize();
        
        AnimateFruit(true);
    }

    private void AnimateFruit(bool isIn)
    {
        if (animateCoroutine != null)
        {
            StopCoroutine(animateCoroutine);
        }
        animateCoroutine = StartCoroutine(AnimateCoroutine(isIn));
    }

    private IEnumerator AnimateCoroutine(bool isIn)
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = isIn ? Vector3.zero : Vector3.one;

        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / animTime);
            yield return null;
        }

        fireImage.gameObject.SetActive(false);
        
        transform.localScale = endScale;
    }
}
