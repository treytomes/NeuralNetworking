Two networks:
1. An action network (A).
2. An error signal/judgement network (E).

(A) receives visual input from the environment, which outputs into an action:
1. Turn left/right (-1/1).
2. Move backward/forward (-1/1).

(E) receives the same visual input that (A) just processed, along with (A)'s output signal.  The output of (E) is an error signal which is then fed back into (A) to adjust (A)'s weights for the next move.

The goal of (E) is to minimize the time and number of steps to reach the goal.

* How is (E) trained?
* How does (E) track time?
* How does (E) track the number of steps?
