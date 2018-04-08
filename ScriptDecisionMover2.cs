using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptDecisionMover2 : MonoBehaviour {
	private static List<Vector3> conditionals;
	public static Vector3 ideal_center = Vector3.zero;
	private static int decision_count = 5;

	private const float speed_base = 30f;

	private const float alpha_conditional = 0.999f;
	private const float alpha_ideal_center_decay = 1.000f;
	private const float alpha_ideal_center = 0.999f;
	private const float speed_bonus_escape = 5f;
	private const float rotation_scalar = 4f;

	Renderer r;
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		Vector3 heading = ideal_center - transform.localPosition;

		float align_counter = 1f;

		float s = speed_base;
		for (int i = 0; i < decision_count; i++) {
			float residual = Vector3.Dot (conditionals [i], transform.localPosition - ideal_center);
			float sign = Mathf.Sign (residual);
			float c = Mathf.Clamp01(Vector3.Dot (transform.forward, conditionals[i].normalized));

			// head in direction described by conditionals[i]
			heading = heading + sign * conditionals[i].normalized;

			// interpolate between the tangent vector "conditionals[i]" which descibes a separating plane,
			// and the tangent vector which would put us on the opposite side of the separating plane
			// optimal vector of partial derivatives to change sign of DOT(tangent [x,y,z], position [x,y,z]) = -1f * sign * transform.forward
			conditionals [i] = alpha_conditional * conditionals [i] - (1 - alpha_conditional) * sign * c * transform.forward;

			// move faster when misaligned with heading
			s += (1 - c) * speed_bonus_escape;
			align_counter += Mathf.Max (sign, 0f);
			}

		float h = Mathf.Clamp(Vector3.Dot(transform.forward, heading.normalized), 0.1f, 1f);

		// decay toward Vector3.zero, but offset slightly by a point behind self
		ideal_center = alpha_ideal_center * alpha_ideal_center_decay * ideal_center + (1 - alpha_ideal_center) * (transform.localPosition);

		// rotate toward heading described by plane-separator
		// rotate faster when misaligned, rotate slower when aligned
		Vector3 t = Vector3.RotateTowards (transform.forward, heading, Time.deltaTime * rotation_scalar * (1 - h) / align_counter, 0f);
		transform.LookAt (transform.localPosition + t);
		transform.localPosition += transform.forward * Time.deltaTime * s;
	}

	public static void Populate(GameObject prefab_reference, Transform t, int q) {
		conditionals = new List<Vector3> ();
		for (int i = 0; i < decision_count; i++) {
			conditionals.Add (new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), Random.Range (-1f, 1f)));
		}

		for (int d = 0; d < q; d++)
			Instantiate (prefab_reference, new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), Quaternion.Euler (Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f)), t);
	}

}
