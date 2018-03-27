#include <stdlib.h>
#include <math.h>

#ifdef __APPLE__
#include <GLUT/glut.h>
#else
#include <GL/glut.h>
#include <iostream>

#endif

// angle of rotation for the camera direction
float angle = 0.0f;
// actual vector representing the camera's direction
float lx=0.0f,lz=-1.0f;
// XZ position of the camera
float x=0.0f, z=5.0f;

float ux = 1, uy = 0;
// the key states. These variables will be zero
//when no key is being presses
float deltaAngle = 0.0f;
float deltaMove = 0;
float deltaTheta = 0;
float theta = 0;
float cubeTheta = 0;
float cubePhi = 0;
float dCubeTheta = 0;
float dCubePhi = 0;

void randomColor()
{
    float r = rand()%255, g = rand()%255, b = rand()%255;
    r /= 255;
    g /= 255;
    b /= 255;
    glColor3f(r, g, b);
}

void cubeHelper()
{
    glBegin(GL_QUADS);
    randomColor();
    glVertex3f(-0.5f, -0.5f, 0.5f);
    glVertex3f(0.5f, -0.5f, 0.5f);
    glVertex3f(0.5f, 0.5f, 0.5f);
    glVertex3f(-0.5f, 0.5f, 0.5f);
    randomColor();
    glVertex3f(-0.5f, -0.5f, -0.5f);
    glVertex3f(0.5f, -0.5f, -0.5f);
    glVertex3f(0.5f, 0.5f, -0.5f);
    glVertex3f(-0.5f, 0.5f, -0.5f);
    glEnd();
}

void cube(float theta, float phi)
{
    glPushMatrix();
    glRotatef(phi, 1, 0 ,0);
    glRotatef(theta, 0, 1, 0);

    cubeHelper();
    glPushMatrix();
    glRotatef(90, 0, 1, 0);
    cubeHelper();
    glPopMatrix();
    glPushMatrix();
    glRotatef(90, 1, 0, 0);
    cubeHelper();
    glPopMatrix();


    glPopMatrix();
}

void changeSize(int w, int h) {

    // Prevent a divide by zero, when window is too short
    // (you cant make a window of zero width).
    if (h == 0)
        h = 1;
    float ratio =  w * 1.0 / h;

    // Use the Projection Matrix
    glMatrixMode(GL_PROJECTION);

    // Reset Matrix
    glLoadIdentity();

    // Set the viewport to be the entire window
    glViewport(0, 0, w, h);

    // Set the correct perspective.
    gluPerspective(45.0f, ratio, 0.1f, 100.0f);

    // Get Back to the Modelview
    glMatrixMode(GL_MODELVIEW);
}

void computePos(float deltaMove) {

    x += deltaMove * lx * 0.1f;
    z += deltaMove * lz * 0.1f;
}

void computeDir(float deltaAngle, float deltaTheta) {

    angle += deltaAngle;
    lx = sin(angle);
    lz = -cos(angle);

    theta += deltaTheta;
    ux = sin(theta);
    uy = -cos(theta);

    std::cout << ux << " " << uy << std::endl;
}


void renderScene(void) {
    srand(0);
    if (deltaMove)
        computePos(deltaMove);
    if (deltaAngle || deltaTheta)
        computeDir(deltaAngle, deltaTheta);

    cubeTheta += dCubeTheta;
    cubePhi += dCubePhi;
    // Clear Color and Depth Buffers
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

    // Reset transformations
    glLoadIdentity();
    // Set the camera
    gluLookAt(	x, 1.0f, z,
                  x+lx, 1.0f,  z+lz,
                  ux, uy, 0);

    cube(cubePhi, cubeTheta);
    glutSwapBuffers();
}


void pressKey(int key, int xx, int yy) {

    switch (key) {
        case GLUT_KEY_LEFT : deltaAngle = -0.01f; break;
        case GLUT_KEY_RIGHT : deltaAngle = 0.01f; break;
        case GLUT_KEY_UP : deltaMove = 0.5f; break;
        case GLUT_KEY_DOWN : deltaMove = -0.5f; break;
        case GLUT_KEY_HOME: deltaTheta = 0.01f; break;
        case GLUT_KEY_END: deltaTheta = -0.01; break;
    }
}

void pressKKey(unsigned char key, int xx, int yy) {
    printf("%c\n", key);
    switch (key) {
        case 'q':
            dCubePhi = 1;
            return;
        case 'e':
            dCubePhi = -1;
            return;
        case 'a':
            dCubeTheta = 1;
            return;
        case 'd':
            dCubeTheta = -1;
            return;

    }
}

void releaseKKey(unsigned char key, int xx, int yy) {
    printf("%c\n", key);
    switch (key) {
        case 'q':
        case 'e':
            dCubePhi = 0;
            return;
        case 'a':
        case 'd':
            dCubeTheta = 0;
            return;

    }
}



void releaseKey(int key, int x, int y) {

    switch (key) {
        case GLUT_KEY_LEFT :
        case GLUT_KEY_RIGHT : deltaAngle = 0.0f;break;
        case GLUT_KEY_UP :
        case GLUT_KEY_DOWN : deltaMove = 0;break;
        case GLUT_KEY_HOME:
        case GLUT_KEY_END: deltaTheta = 0; break;
    }
}

int main(int argc, char **argv) {

    // init GLUT and create window
    glutInit(&argc, argv);
    glutInitDisplayMode(GLUT_DEPTH | GLUT_DOUBLE | GLUT_RGBA);
    glutInitWindowPosition(100,100);
    glutInitWindowSize(320,320);
    glutCreateWindow("");

    // register callbacks
    glutDisplayFunc(renderScene);
    glutReshapeFunc(changeSize);
    glutIdleFunc(renderScene);

    glutSpecialFunc(pressKey);
    glutKeyboardFunc(pressKKey);
    glutKeyboardUpFunc(releaseKKey);
    // here are the new entries
    glutIgnoreKeyRepeat(1);
    glutSpecialUpFunc(releaseKey);

    // OpenGL init
    glEnable(GL_DEPTH_TEST);

    // enter GLUT event processing cycle
    glutMainLoop();

    return 1;
}