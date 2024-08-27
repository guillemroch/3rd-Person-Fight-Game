using UnityEngine;

public class ArrowShooters : MonoBehaviour{
    public bool shoot;
    public GameObject arrow;
    public Vector3 direction;
    public float strength;

    public float time = 1f;
    public float timer;
    void Start()
    {
       shoot = true;
       timer = 0f;
    }

    void Update()
    {
        if (shoot) {
            ShootArrow();
        }

        timer += Time.deltaTime;
        if (timer > time) {
            shoot = true;
            timer = 0;
        }

    }

    public void ShootArrow() {
        GameObject t = Instantiate(arrow, transform.position, Quaternion.identity);
        t.gameObject.GetComponent<ArrowController>().SetVelocity(transform.forward * strength);
        shoot = false;

    }
    
}
