# UnityFlocker
Flocking behavior using separating planes

See https://youtu.be/9WQR-t7s23A

The magic happens in these two lines:

			heading = heading + sign * conditionals[i].normalized;
			conditionals [i] = alpha_conditional * conditionals [i] - (1 - alpha_conditional) * sign * c * transform.forward;
      
To achieve flocking behavior, we divide the game world using n planes. Actors are either above or below each plane. Then we can issue similar orders to every actor depending upon whether they are above or below our separating planes, instead of issuing orders to nearby members of the flock.

The actor's future heading is a composite of its present heading, and the heading described by a vector shared by the flock. This gives us groups of actors which have similar behavior.

We also need to continuously agitate the flock. The planes can be manipulated to always be turning in order to exclude the actors that are presently above them. This auto-balances the number of actors above the plane toward 50% of the population. The center of the flock is shifted to constantly be behind the heading of the mass of the flock, so the flock's front is continuously turning around, while stragglers are driven to a point somewhere behind the flock.
