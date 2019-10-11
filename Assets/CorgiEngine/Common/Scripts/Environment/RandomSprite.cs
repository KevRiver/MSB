using UnityEngine;
using System.Collections;

namespace MoreMountains.CorgiEngine
{	
	[RequireComponent(typeof(SpriteRenderer))]

	/// <summary>
	/// This component will randomize the object's sprite renderer's sprite out of a collection
	/// </summary>
	public class RandomSprite : MonoBehaviour
	{
		/// the collection of sprite to choose from
	    public Sprite[] SpriteCollection;

	    protected SpriteRenderer _spriteRenderer;

	    /// <summary>
	    /// On Start, initializes the renderer and randomizes it
	    /// </summary>
	    protected virtual void Start()
	    {
			_spriteRenderer = GetComponent<SpriteRenderer>();
			Randomize();
	    }

	    /// <summary>
	    /// Returns a random sprite out of the collection
	    /// </summary>
	    protected virtual void Randomize()
	    {
			_spriteRenderer.sprite = SpriteCollection[Random.Range(0, SpriteCollection.Length)];
	    }
	}
}