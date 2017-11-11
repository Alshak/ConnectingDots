using UnityEngine;

public class PlayerEffects : MonoBehaviour {
    public GameObject particleCollisionTemplate;
    public GameObject particleImpactTemplate;

    public void CreateParticlesCollision(RelativePosition mainBlockPosition, RelativePosition secondaryBlockPosition)
    {
        Vector2 particlesPosition = Vector2.zero;
        GameObject particles = null;
        switch (mainBlockPosition)
        {
            case RelativePosition.LEFT:
                switch (secondaryBlockPosition)
                {
                    case RelativePosition.LEFT:
                        particlesPosition = new Vector2(transform.position.x - 1.5f, transform.position.y);
                        break;
                    case RelativePosition.TOP:
                        particlesPosition = new Vector2(transform.position.x - 0.5f, transform.position.y);
                        break;
                    case RelativePosition.RIGHT:
                        particlesPosition = new Vector2(transform.position.x - 0.5f, transform.position.y);
                        break;
                    case RelativePosition.BOTTOM:
                        particlesPosition = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
                        break;
                }
                particles = Instantiate(particleCollisionTemplate, particlesPosition, new Quaternion());
                break;
            case RelativePosition.RIGHT:
                switch (secondaryBlockPosition)
                {
                    case RelativePosition.RIGHT:
                        particlesPosition = new Vector2(transform.position.x + 1.5f, transform.position.y);
                        break;
                    case RelativePosition.TOP:
                        particlesPosition = new Vector2(transform.position.x + 0.5f, transform.position.y);
                        break;
                    case RelativePosition.LEFT:
                        particlesPosition = new Vector2(transform.position.x + 0.5f, transform.position.y);
                        break;
                    case RelativePosition.BOTTOM:
                        particlesPosition = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
                        break;
                }
                particles = Instantiate(particleCollisionTemplate, particlesPosition, new Quaternion());
                break;
            case RelativePosition.BOTTOM:
                switch (secondaryBlockPosition)
                {
                    case RelativePosition.BOTTOM:
                        particlesPosition = new Vector2(transform.position.x, transform.position.y - 1.5f);
                        break;
                    case RelativePosition.TOP:
                        particlesPosition = new Vector2(transform.position.x, transform.position.y - .5f);
                        break;
                    case RelativePosition.LEFT:
                        particlesPosition = new Vector2(transform.position.x - .5f, transform.position.y - .5f);
                        break;
                    case RelativePosition.RIGHT:
                        particlesPosition = new Vector2(transform.position.x + .5f, transform.position.y - .5f);
                        break;
                }
                particles = Instantiate(particleImpactTemplate, particlesPosition, new Quaternion());
                break;
        }
        Destroy(particles, particles.GetComponent<ParticleSystem>().main.duration);
    }
}
