# Donko
production assignment, 1.5 month dev cycle.  suffering involved.  build available at https://zoruakev.itch.io/donko

Some notes about the code:
- player controller uses a rigidbody.  would have preferred a charactercontroller but it works decently for adding knockback.
- proper animation transitions/blends are used.  surprising lack of blend trees, given our character only moves forwards facing.
- game doesn't use navmeshes.  pros to giving ur enemies literal slug brains.
- the menu stuff is largely ripped straight out of my previous assignment (roomba) with some extra bits pasted on.
  it was made at the end of *that* assignment to, so there's redundant and unclean code.
- shader graphs are a decent reference.  followed a tutorial for the toon shader, water/portal are totally original.
