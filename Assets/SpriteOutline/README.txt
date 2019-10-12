//
// Sprite Outline
// Setup/Usage Instructions
//
// http://u3d.as/10R7 (Asset Store link)
//

For a visual walkthrough of the setup/usage instructions, please refer to the
video on the Asset Store page or watch the video directly on YouTube:
  -> https://youtu.be/B9Euv0VnDDs

----------

[Release Notes]

v2.1   (04/07/19) => Added UI.image support and buffer slider.

v2.0.1 (02/11/19) => Fixed real-time rendering on mobile.

v2.0   (01/14/19) => Major changes/improvements. For additional information,
                     refer to [Upgrade Notes] and [Changelog] below.

v1.1   (05/14/18) => Added spritesheet, animation, and isometric sorting
                     support.

v1.0   (01/02/18) => Initial release.

----------

[Setup]

The only setup step required for using this component is:
 * turning on "Read/Write Enabled" in the Import Settings for each sprite that
   requires an outline

Q: Why is this necessary?
A: In order for the included pixel shader to display outlines around your
   sprites, each sprite must have a border of transparent pixels surrounding it
   that is equal to or greater than the desired size of the outline.

   For example, if a 5px outline is desired, that sprite must have a 5px border
   of transparent pixels surrounding it in all 4 directions (above/below and
   both sides) to allow the outline to be fully visible.

   Rather than having to manually add the aforementioned border to each and
   every sprite that requires an outline, this component does it for you
   automatically via code (by creating a new texture in memory; it does not
   modify the original source texture). To allow the raw pixel data of the
   sprite to be accessed, "Read/Write Enabled" must be turned on.

----------

[Usage]

1. Add the "Sprite Outline" component to a game object that has a sprite which
   needs an outline. (Refer to steps 7-9 and [Known Limitations] for additional
   information regarding child sprites).

2. Drag the "Size" slider to adjust the thickness of the outline.

3. Drag the "Blur Size" slider to add/remove blurring (anti-aliasing) to the
   outline by gradually fading the number of outer edges equivalent to the
   specified value.

4. Use "Color" to change the color of the outline by choosing one from the
   color picker. Additionally, you can adjust the overall opacity of the
   outline by changing the alpha value.

5. Drag the "Blur Alpha Multiplier" slider to adjust the opacity of *only* the
   blurred edges of the outline (if any). This is useful if you would like the
   solid, unblurred edges of the outline to have more contrast against the
   blurred edges.

6. Drag the "Blur Alpha Choke" slider to adjust how quickly the blurred edges
   of the outline fade away.

7. Toggle "Invert Blur" to reverse the fade direction of the blurring (from the
   inside out to the outside in).

8. Drag the "Alpha Threshold" slider to adjust the minimum amount of opacity a
   sprite pixel must have for an outline to be placed around it. This is useful
   if you are using sprites that have semi-transparent pixels in them and want
   those pixels to *not* be included in the outline.

9. Drag the "Buffer" slider to add/remove a buffer of transparent pixels
   between the sprite(s) and the outline.

10. Enable "Include Children" to have child sprites nested within the main
    sprite included in the outline.

11. Use "Child Layers" to filter the child sprites that will be included in the
    outline on a per-layer basis. Child sprites that do *not* belong to one of
    the checked layers will be excluded.

12. Fill "Ignore Child Names" with names of nested game objects that you want
    excluded from the outline.

13. Change "Sort Method" to adjust how the outline is sorted (either the lowest
    sorting order - 1; or the highest z-axis value + 1).

14. Enable "Is Animated" to auto-regenerate the outline when the main sprite
    frame changes (does not track child sprites).

15. Enable "Use Exported Frame" to use a pre-rendered image of the outline
    instead of rendering in real time using the included pixel shader. If your
    outline does not change appearance at all during run time, you should
    enable this before releasing your game as a performance optimization. This
    also improves support/performance on mobile devices.

    NOTE: To generate a pre-rendered outline image, click the "Export" button
    under "Editor Actions" or call the Export() method via code. If you are
    using an older version of Unity, you may need to Alt+Tab out of the Unity
    Editor and back into it for the image to be imported. The path to the
    exported outline is displayed in the console.

16. Use "Custom Frame Name" to change the file name of the exported outline
    from the default (the name of the game object) to one of your choosing.
    This allows multiple game objects sharing the same name to export unique
    outlines by preventing one from overriding the other.

17. Enable "Generates On Start" to auto-regenerate the outline at game start.

18. Disable "Generates On Validate" to prevent auto-regeneration of the outline
    any time the component is loaded in the editor or when any value is changed
    via the Inspector.

----------

[Public Methods]

* Regenerate() -> Updates the outline.

* Export() -> Save a rendered image of the outline to disk. (Refer to [Usage]
              steps 14-15 to see how/why to use this feature.)

* Clear() -> Permanently destroys the outline.

* Hide() -> Makes the outline disappear, without permanently destroying it.

* Show() -> Makes the outline visible, if it was previously hidden.

* SortOutline(float zOffset, int? sortingOrder, int? sortingLayerId)
    -> Sets the z-axis offset, sorting order, and/or layer of the outline
       to the specified value(s).

* ShouldIgnoreSprite(GameObject instance, Sprite sprite) [Overridable]
    -> Callback method used to determine which child sprites to exclude from
       the outline when traversing the children of the main sprite.

----------

[Upgrade Notes]

If you are upgrading from a 1.x version, please read the following notes on
how to do so:

1. Delete the OutlineObject folder entirely (the component is now called
   SpriteOutline).

2. Select all prefabs and/or game objects that were previously using the
   "Outline Object" component (if a game object is an instance of a prefab,
   select only the prefab and not the game object itself).

3. Scroll down in the Inspector until you see an unnamed component that states
   a script is missing (this is where the "Outline Object" component was
   previously attached). The missing script will appear as:

   (Script)

   Script: Missing (Mono Script)

   The associated script cannot be loaded.
   Please fix any compile errors
   and assign a valid script.

4. Assign the "Sprite Outline" component to this script by clicking on the icon
   to the right of the "Missing (Mono Script)" box and selecting it from the
   window that appears.

5. Finally, select all game objects that are instances of prefabs and click the
   "Revert" button near the top of the Inspector to properly restore all values
   of the component to their previous state.

----------

[Changelog]

v2.1:

* ADDED:
   -> UI.Image support
   -> buffer

v2.0:

* RENAMED:
   -> OutlineObject to SpriteOutline
   -> outlineSize/outlineBlur/outlineColor to size/blurSize/color

* ADDED:
   -> multi-object editing support
   -> blurAlphaMultiplier
   -> blurAlphaChoke
   -> invertBlur
   -> alphaThreshold
   -> ignoreChildNames
   -> sortMethod
   -> useExportedFrame
   -> customFrameName
   -> generatesOnValidate

* REMOVED:
   -> outlineMaterial (now using Shader.Find to automatically set this)
   -> isIsometric (replaced with sortMethod)
   -> childrenOverlap (no longer necessary)
   -> SetSortOrder()/SetSortOrderOffset() (use SortOutline() instead)

* MISC:
   -> increased the max outline size/blurSize to 20px/19px instead of 10px/9px

----------

[Known Limitations]

* Rotation/scaling is only supported on the main sprite, not children. If you
  need to rotate or scale a child sprite that requires an outline, you should
  instead add individual "Sprite Outline" components to each sprite with
  "Include Children" disabled.

* Outlines can only be exported for non-animated sprites.

* Outlines are exported to the directory: "Assets/SpriteOutline/Resources/". If
  you installed this asset to a different path and would like to use the Export
  feature, you will need to edit the following line in "SpriteOutline.cs" to
  match the path you are using:

  public const string RESOURCE_DIR = "Assets/SpriteOutline/Resources/";

  NOTE: The path *must* end with "/Resources/" to work correctly (so that the
  exported outlines can be loaded at run time via code).

----------

Thanks for reading!

Have questions? Send an email to: support@legit-games.com


