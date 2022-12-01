using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public int inc;
    public ScoreSystem ss;
    public Transform target;
    public SimpleShoot simsho;

    void Start()
    {
        StartCoroutine(Attacking());
        target = GameObject.FindWithTag("Player").GetComponent<Transform>();
        ss = GameObject.FindWithTag("score").GetComponent<ScoreSystem>();
    }

    void Update()
    {
        transform.LookAt(target);
    }

    public void scoreUpdater()
    {
        ss.updateScore(inc);
    }

    IEnumerator Attacking()
    {
        Material mymat = GetComponent<Renderer>().material;
        mymat.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(4);
        mymat.SetColor("_EmissionColor", Color.yellow);
        yield return new WaitForSeconds(4);
        mymat.SetColor("_EmissionColor", Color.red);
        yield return new WaitForSeconds(4);
        simsho.ShootAnim();
        yield return new WaitForSeconds(1);
        GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().LoadScene("DeathScreen");
    }
}
