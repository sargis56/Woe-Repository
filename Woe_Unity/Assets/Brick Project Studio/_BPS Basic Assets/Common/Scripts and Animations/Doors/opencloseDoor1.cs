using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SojaExiles

{
	public class opencloseDoor1 : NetworkBehaviour
    {

		public Animator openandclose1;
		public bool open;
		public GameObject[] players;
		Material defaultMaterial;
		public Material selectMaterial;
		bool highLight;
		public override void OnNetworkSpawn()
        {
            open = false;
            //players = GameObject.FindGameObjectsWithTag("Player");
			defaultMaterial = this.GetComponent<MeshRenderer>().material;
			highLight = true;
		}

        void OnMouseOver()
		{
			players = GameObject.FindGameObjectWithTag("Director").GetComponent<GameController>().players;

			if (IsSpawned) {
                foreach (GameObject player in players)
                {
                    float dist = Vector3.Distance(player.transform.position, transform.position);
					if (dist < 5) //originally 15
					{
						
						if (highLight)
						{
							this.GetComponent<MeshRenderer>().material = selectMaterial;
						}
						if (open == false)
						{
							if (Input.GetKeyDown("e"))
							{
								highLight = false;
								StartCoroutine(opening());
							}
						}
						else
						{
							if (open == true)
							{
								if (Input.GetKeyDown("e"))
								{
									highLight = false;
									StartCoroutine(closing());
								}
							}

						}

					}
					else
                    {
						this.GetComponent<MeshRenderer>().material = defaultMaterial;
					}
				}
				
			}

		}

		IEnumerator opening()
		{
			print("you are opening the door");
			openandclose1.Play("Opening 1");
			open = true;
			this.GetComponent<MeshRenderer>().material = defaultMaterial;
			yield return new WaitForSeconds(.5f);
			highLight = true;
		}

		IEnumerator closing()
		{
			print("you are closing the door");
			openandclose1.Play("Closing 1");
			open = false;
			this.GetComponent<MeshRenderer>().material = defaultMaterial;
			yield return new WaitForSeconds(.5f);
			highLight = true;
		}


	}
}