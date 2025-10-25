#include <iostream>
using namespace std;

double CircleSquare()
{
    double radius = 12;
    double square = 3.14 * radius * radius;
    return square;
}

int main()
{
    double answer = CircleSquare();
    cout << "Square of circle: " << answer << endl;
    return 0;
}