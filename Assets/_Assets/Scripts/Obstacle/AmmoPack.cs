using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoPack : MonoBehaviour, IHittable, IFallingObstacle {
    [SerializeField] private float fallingSpeed = 0.1f;
    [SerializeField] private int ammoAmount = 0;
    [SerializeField] private TextMeshProUGUI[] texts;

    private Vector3 launchVector;


    private void Start() {
        SetTexts();
    }

    private void SetTexts() {
        foreach (TextMeshProUGUI text in texts) {
            text.text = "+" + ammoAmount;
        }
    }

    public HittableType GetHittableType() {
        return HittableType.Obstacle;
    }

    public void Hit(BaseProjectile projectile, Collision collision) {
        if (projectile.GetHittableType() == HittableType.PlayerProjectile) {
            Player.Instance.AddAmmo(ammoAmount);
            EffectHandler.Instance.SpawnTextEffect("+" + ammoAmount, transform.position, TextEffect.TextColor.Orange, 0.4f, 1.5f);
        }
        DestroyObstacle();
    }



    private void FixedUpdate() {
        transform.position -= new Vector3(0, fallingSpeed * Time.deltaTime, 0);
        if (transform.position.y < -10) {
            DestroyObstacle();
        }
    }

    public int GetDamage() {
        return 0;
    }
    public float GetFallSpeed() {
        return fallingSpeed;
    }

    public void SetFallSpeed(float speed) {
        fallingSpeed = speed;
    }
    public void SetInverted(bool i) {
    }
    public bool IsInverted() {
        return false;
    }

    public void SetAmmoAmount(int amount) {
        ammoAmount = amount;
        SetTexts();
    }
    public int GetAmmoAmount() {
        return ammoAmount;
    }
    private void DestroyObstacle() {
        ObstacleSpawner.Instance.RemoveObstacleFromList(this.transform);
        Destroy(gameObject);
    }
}
