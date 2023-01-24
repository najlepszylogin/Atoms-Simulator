# Atoms-Simulator
Base for learning about Atoms and Particles written in C#/Unity.

Unlicensed song is included in original project - there is no music in this version.

The editor allows you to model atoms by adding/subtracting neutrons, protons and electrons to
basics. Following the original scheme for creating an effective construction atom z
available in its memory and comes to a successful state, or an explosion in the event of
mismatches.

<img src="https://user-images.githubusercontent.com/46264442/214274416-fe1a18ec-a5f3-4482-b0b2-af46306c9908.png" width="480">


Atom's class is builded by 4 the most important fields and dozen of others:
- number of protons (tells the algorithm when it can be a real atom)
- number of neutrons (tells the algorithm when it can be a real atom)
- array of number of electrons (7) 
- period (chemical)

At this moment editor can recgonise 11 atoms:

<img src="https://user-images.githubusercontent.com/46264442/214275737-4110c206-3303-40a0-815e-e1ed7ad60ada.png" width="480">

Atom recgonise algorithm:

<img src="https://user-images.githubusercontent.com/46264442/214275890-3c185921-fc36-4a01-b611-0fb89fabfa7f.png" width="480">

<hr>
<h2> Particle Editor</h2>

The editor allows you to model molecules by adding the appropriate atoms and creating between them
connections. After constructing the appropriate structure, the editor compares the constructed molecule
with those available in its memory and displays its name at the top of the screen in case of a match.

<img src="https://user-images.githubusercontent.com/46264442/214276307-51b96db3-d70e-4831-9128-4047412614a8.png" width="480">

Particle is storing much more informations:
- Objects of Atoms on the screen
- Connections between them
- Number of Atoms and Connections
- Number of connected atoms


Currentlythere is declared 8 of real particles that can be created in simulator:
<img src="https://user-images.githubusercontent.com/46264442/214277162-312e7deb-2468-49d3-b739-6662b70930d1.png" width="480">

Algorithm of particle recgonise is just checking every connection and particle.

