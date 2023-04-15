using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SojaExiles

{
	public class opencloseDoor : NetworkBehaviour
    {

		public Animator openandclose;
		public bool open;
		public GameObject[] players;

        public override void OnNetworkSpawn()
        {
            open = false;
            players = GameObject.FindGameObjectsWithTag("Player");
        }

		void OnMouseOver()
		{
			if (IsOwner) {
				foreach (GameObject player in players)
				{
					float dist = Vector3.Distance(player.transform.position, transform.position);
					if (dist < 15)
					{
						if (open == false)
						{
							if (Input.GetKeyDown("e"))
							{
								StartCoroutine(opening());
							}
						}
						else
						{
							if (open == true)
							{
								if (Input.GetKeyDown("e"))
								{
									StartCoroutine(closing());
								}
							}

						}

					}
				}

			}

		}

		IEnumerator opening()
		{
			print("you are opening the door");
			openandclose.Play("Opening");
			open = true;
			yield return new WaitForSeconds(.5f);
		}

		IEnumerator closing()
		{
			print("you are closing the door");
			openandclose.Play("Closing");
			open = false;
			yield return new WaitForSeconds(.5f);
		}


	}
}