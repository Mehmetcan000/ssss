using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OyuncuHareketManager : MonoBehaviour
{
    // Burada animasyon kontrol� i�in bir bool de�i�keni atad�k ve ona true false vererek hareket ettirdik.
    bool hareketlimi;
    //Hangi Y�ne gidecegini kararlastirmasi icin bir vector3 tan�mlad�k.
    Vector3 hangiYon;
    //Quaternion metodu
    Quaternion donusYon;
    //Animatorden anim adli degisken tan�mladik
    Animator anim;
    
    public LayerMask engelLayer; // Engellerin yer aldığı katman

    public GameObject particleEffectPrefab;
    private void Awake()
    {
        anim = GetComponent<Animator>(); 
    } 
    public void HareketEt(Vector3 hedefPos, float gecikmezamani=0.25f)
    {  
        if (!hareketlimi)//Hareketli degil ise calisacak if blogu
        {   
            StartCoroutine(HareketRoutine(hedefPos, gecikmezamani));
        }
    }
    IEnumerator HareketRoutine(Vector3 hedefPos, float gecikmezamani)
    {
        hareketlimi = true;

        hangiYon = new Vector3(hedefPos.x -transform.position.x, transform.position.y,hedefPos.z - this.transform.position.z);

        donusYon = Quaternion.LookRotation(hangiYon);

        transform.DORotateQuaternion(donusYon, .3f);

        anim.SetBool("HareketEtsinmi", true);

        yield return new WaitForSeconds(.3f);

        this.transform.DOMove(hedefPos, gecikmezamani);

        while (Vector3.Distance(hedefPos, this.transform.position) < 0.01f)
        {
            yield return null;
        }
        // Raycast kontrolü ekledik
        if (Physics.Raycast(transform.position, hangiYon.normalized, out RaycastHit hit, Vector3.Distance(transform.position, hedefPos), engelLayer))
        {
            Debug.Log("Engel var: " + hit.collider.name);
            anim.SetBool("HareketEtsinmi", false);
            hareketlimi = false;
            yield break;
        }
        anim.SetBool("HareketEtsinmi", false);

        donusYon = Quaternion.LookRotation(Vector3.zero);

        transform.DORotateQuaternion(donusYon, .3f);

        this.transform.position = hedefPos;
        ShowParticleEffect(hedefPos);

        hareketlimi=false;
    }
    void ShowParticleEffect(Vector3 position)
    {
        GameObject particleEffect = Instantiate(particleEffectPrefab, position, Quaternion.identity);
        Destroy(particleEffect, 2.5f); // 2 saniye sonra particle effect'i yok et
    }
    public void OyuncuHataYapti()
    {
        anim.SetBool("hataYapti", true);
    }

    public void OyuncuGeriGelsin()
    {
        anim.SetBool("hataYapti", false);
        this.transform.position = Vector3.zero;
    }


    
}
