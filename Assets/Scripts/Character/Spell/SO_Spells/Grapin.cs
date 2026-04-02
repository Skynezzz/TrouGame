using System;
using System.Collections;
using System.Linq;
using TrouGame.Miscellaneous;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace TrouGame.Character.Spell.Spells
{
    [CreateAssetMenu(fileName = "Grapin", menuName = "Spell/Grapin")]
    public class Grapin : SpellBase
    {
        [Header("Graplin")]
        [SerializeField] GameObject grapIndicatorPrefab;
        [SerializeField] float maxRange = 20f;
        [SerializeField] float minRange = 3f;
        [SerializeField] float speed = 100f;
        [SerializeField] float maxGrapPullTime = 5f;

        Transform lookAt => Anchor.TRANSFORMS["LookAt"];
        Coroutine grapPullCoroutine;
        GameObject grapIndicator;
        Image grapIndicatorImage => grapIndicator.GetComponent<Image>();

        public override void Select()
        {
            grapIndicator = Instantiate(grapIndicatorPrefab, FindAnyObjectByType<Canvas>().transform);
        }

        public override void UpdateOnSelect()
        {
            if (Physics.Raycast(lookAt.position, lookAt.forward, out RaycastHit hit, maxRange))
            {
                if (hit.distance > minRange)
                {
                    grapIndicator.SetActive(true);

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Entity"))
                    {
                        if (hit.collider.attachedRigidbody.mass < spellManager.GetComponent<Rigidbody>().mass)
                        {
                            grapIndicatorImage.color = Color.green;
                        }
                        else
                        {
                            grapIndicatorImage.color = Color.red;
                        }
                    }
                    else
                    {
                        grapIndicatorImage.color = Color.white;
                    }
                }
                else
                {
                    grapIndicator.SetActive(false);
                }
            }
            else
            {
                grapIndicator.SetActive(false);
            }
        }

        public override void UnSelect()
        {
            if (grapPullCoroutine != null)
            {
                spellManager.StopCoroutine(grapPullCoroutine);
                grapPullCoroutine = null;
            }
            Destroy(grapIndicator);
        }

        public override void Active()
        {
            if (grapPullCoroutine != null)
            {
                spellManager.StopCoroutine(grapPullCoroutine);
                grapPullCoroutine = null;
            }
            else if (Physics.Raycast(lookAt.position, lookAt.forward, out RaycastHit hit, maxRange))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Entity"))
                {
                    if (hit.collider.attachedRigidbody.mass < spellManager.GetComponent<Rigidbody>().mass)
                    {
                        grapPullCoroutine = spellManager.StartCoroutine(GrapPull(() => lookAt.position, hit.collider.transform));
                    }
                    else
                    {
                        grapPullCoroutine = spellManager.StartCoroutine(GrapPull(() => hit.collider.transform.position, spellManager.transform));
                    }
                }
                else
                {
                    grapPullCoroutine = spellManager.StartCoroutine(GrapPull(() => hit.point, spellManager.transform));
                }
            }
        }

        private IEnumerator GrapPull(Func<Vector3> GetHookPosition, Transform target)
        {
            float startTime = Time.time;
            Rigidbody rigidbody = target.GetComponent<Rigidbody>();

            while (Vector3.Distance(target.position, GetHookPosition()) > minRange && Time.time - startTime < maxGrapPullTime && GetHookPosition() != Vector3.positiveInfinity)
            {
                rigidbody.linearVelocity = (GetHookPosition() - target.position).normalized * speed;
                yield return null;
            }

            grapPullCoroutine = null;
        }
    }
}