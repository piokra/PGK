#include <stdlib.h>
#include <math.h>
#include <png.h>
#ifdef __APPLE__
#include <GLUT/glut.h>
#else

#include <GL/glut.h>
#include <iostream>

#include "textura1.c"
#include "textura2.c"

#endif

GLuint textures[2];

// angle of rotation for the camera direction
float angle = 0.0f;
// actual vector representing the camera's direction
float lx = 0.0f, lz = -1.0f;
// XZ position of the camera
float x = 0.0f, z = 5.0f;

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
float mX = 1;
float mY = 1;
float dmX = 0;
float dmY = 0;
float qZ = 0;
float qW = 0;
int screenW = 320, screenH = 320;
bool doRotation = false;

float lightTime = 0;
float light1Pos[4] = {0,0,0,1}, light2Pos[4] = {0,0,0,1}, light3Pos[4] = {0,0,0,1};
float red[4] = {1,0,0,1}, green[4] = {0,1,0,1}, blue[4] = {0,0,1,1};


void drawOneLightObject(float* where) {
    glPushMatrix();
    glTranslatef(where[0], where[1], where[2]);
    glutSolidCube(0.1);
    glPopMatrix();
}

void refreshLightPos() {
    lightTime += 0.01;
    light1Pos[0] = 10*cos(lightTime);
    light1Pos[1] = 10*sin(lightTime);
    light1Pos[2] = 10*sin(cubeTheta);

    light2Pos[0] = 10*sin(lightTime);
    light2Pos[1] = 10*sin(cubeTheta);
    light2Pos[2] = 10*cos(lightTime);

    light3Pos[0] = 10*sin(cubeTheta);
    light3Pos[1] = 10*cos(lightTime);
    light3Pos[2] = 10*sin(lightTime);
}

void drawLightObjects() {

    drawOneLightObject(light1Pos);
    drawOneLightObject(light2Pos);
    drawOneLightObject(light3Pos);

    glLightfv(GL_LIGHT1, GL_POSITION, light1Pos);
    glLightfv(GL_LIGHT2, GL_POSITION, light2Pos);
    glLightfv(GL_LIGHT3, GL_POSITION, light3Pos);

}

void randomColor() {
    float r = rand() % 255, g = rand() % 255, b = rand() % 255;
    r /= 255;
    g /= 255;
    b /= 255;
    glColor3f(r, g, b);
}

void cubeHelper(int face) {
    glBegin(GL_QUADS);
    //randomColor();
    float h = 1./6.;

    glNormal3f(-0.1,-0.1,0.8);
    glTexCoord2d(face*h, 0);
    glVertex3f(-0.5f, -0.5f, 0.5f);

    glNormal3f(0.1,-0.1,0.8);
    glTexCoord2d((face+1)*h, 0);

    glNormal3f(0.1,0.1,0.8);
    glVertex3f(0.5f, -0.5f, 0.5f);
    glTexCoord2d((face+1)*h, 1);

    glNormal3f(-0.1,0.1,0.8);
    glVertex3f(0.5f, 0.5f, 0.5f);
    glTexCoord2d((face)*h, 1);
    glVertex3f(-0.5f, 0.5f, 0.5f);

    ++face;
    //randomColor();
    glNormal3f(0.1,0.1,-0.8);
    glTexCoord2d(face*h, 0);
    glVertex3f(-0.5f, -0.5f, -0.5f);

    glNormal3f(-0.1,0.1,-0.8);
    glTexCoord2d((face+1)*h, 0);
    glVertex3f(0.5f, -0.5f, -0.5f);

    glNormal3f(-0.1,-0.1,-0.8);
    glTexCoord2d((face+1)*h, 1);
    glVertex3f(0.5f, 0.5f, -0.5f);


    glNormal3f(0.1,-0.1,-0.8);
    glTexCoord2d((face)*h, 0);
    glVertex3f(-0.5f, 0.5f, -0.5f);
    glEnd();
}

void cube(float theta, float phi) {
    glPushMatrix();
    glRotatef(phi, 1, 0, 0);
    glRotatef(theta, 0, 1, 0);
    glBindTexture(GL_TEXTURE_2D, textures[0]);
    cubeHelper(0);
    glPushMatrix();
    glRotatef(90, 0, 1, 0);
    cubeHelper(2);
    glPopMatrix();
    glPushMatrix();
    glRotatef(90, 1, 0, 0);
    cubeHelper(4);
    glPopMatrix();


    glPopMatrix();
}

void quaterionRotation(float x, float y, float z, float w) {
    float l = sqrt(x * x + y * y + z * z + w * w);
    float a = x / l, b = y / l, c = z / l, d = w / l;
    float quatMat[] =
            {
                    a * a + b * b - c * c - d * d, 2 * b * c - 2 * a * d, 2 * b * d + 2 * a * c, 0,
                    2 * b * c + 2 * a * d, a * a - b * b + c * c - d * d, 2 * c * d - 2 * a * b, 0,
                    2 * b * d - 2 * a * c, 2 * c * d + 2 * a * b, a * a - b * b - c * c + d * d, 0,
                    0, 0, 0, 1

            };
    glMultMatrixf(quatMat);
}

void quaterionCube(float x, float y, float z, float w) {
    glPushMatrix();
    quaterionRotation(x, y, z, w);
    cube(0, 0);
    glPopMatrix();
}

void changeSize(int w, int h) {

    // Prevent a divide by zero, when window is too short
    // (you cant make a window of zero width).
    if (h == 0)
        h = 1;
    float ratio = w * 1.0 / h;

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
    refreshLightPos();
    if (deltaMove)
        computePos(deltaMove);
    if (deltaAngle || deltaTheta)
        computeDir(deltaAngle, deltaTheta);
    if (doRotation) {
        cubePhi += dmX;
        cubeTheta += dmY;
        qZ += dmY/10;
        qW += dmX/10;
        if (qZ > 1)
            qZ = 0;
        if (qW > 1)
            qW = 0;
    }
    cubeTheta += dCubeTheta;
    cubePhi += dCubePhi;
    // Clear Color and Depth Buffers
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

    // Reset transformations
    glLoadIdentity();
    // Set the camera
    gluLookAt(x, 1.0f, z,
              x + lx, 1.0f, z + lz,
              ux, uy, 0);

    drawLightObjects();
    cube(cubePhi, cubeTheta);
    glTranslatef(1, 1, 0);
    float xx = mX - screenW / 2, yy = mY - screenH / 2;
    quaterionCube(3*xx/screenW, 3*yy/screenH, qZ, qW);
    glutSwapBuffers();
}


void pressKey(int key, int xx, int yy) {

    switch (key) {
        case GLUT_KEY_LEFT :
            deltaAngle = -0.01f;
            break;
        case GLUT_KEY_RIGHT :
            deltaAngle = 0.01f;
            break;
        case GLUT_KEY_UP :
            deltaMove = 0.5f;
            break;
        case GLUT_KEY_DOWN :
            deltaMove = -0.5f;
            break;
        case GLUT_KEY_HOME:
            deltaTheta = 0.01f;
            break;
        case GLUT_KEY_END:
            deltaTheta = -0.01;
            break;
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
        case GLUT_KEY_RIGHT :
            deltaAngle = 0.0f;
            break;
        case GLUT_KEY_UP :
        case GLUT_KEY_DOWN :
            deltaMove = 0;
            break;
        case GLUT_KEY_HOME:
        case GLUT_KEY_END:
            deltaTheta = 0;
            break;
    }
}


void mouseMotion(int x, int y) {
    dmX = mX - x;
    dmY = mY - y;
    mX = x;
    mY = y;
    doRotation = true;
}

void mouseFunc(int, int state, int, int)
{
    if (state == GLUT_UP)
        doRotation = false;
}
int main(int argc, char **argv) {

    // init GLUT and create window
    glutInit(&argc, argv);
    glutInitDisplayMode(GLUT_DEPTH | GLUT_DOUBLE | GLUT_RGBA);
    glutInitWindowPosition(100, 100);
    glutInitWindowSize(320, 320);
    glutCreateWindow("");

    // register callbacks
    glutDisplayFunc(renderScene);
    glutReshapeFunc(changeSize);
    glutIdleFunc(renderScene);

    glutSpecialFunc(pressKey);
    glutKeyboardFunc(pressKKey);
    glutKeyboardUpFunc(releaseKKey);
    glutMouseFunc(mouseFunc);
    // here are the new entries
    glutIgnoreKeyRepeat(1);
    glutSpecialUpFunc(releaseKey);
    glutMotionFunc(mouseMotion);
    // OpenGL init
    glEnable(GL_DEPTH_TEST);
    glEnable(GL_LIGHTING);
    glEnable(GL_LIGHT0);
    glEnable(GL_LIGHT1);
    glEnable(GL_LIGHT2);
    glEnable(GL_LIGHT3);
    glEnable(GL_TEXTURE_2D);
    glEnable(GL_RESCALE_NORMAL);

    glLightfv(GL_LIGHT1, GL_DIFFUSE, red);
    glLightfv(GL_LIGHT2, GL_DIFFUSE, green);
    glLightfv(GL_LIGHT3, GL_DIFFUSE, blue);

    glLightfv(GL_LIGHT1, GL_SPECULAR, red);
    glLightfv(GL_LIGHT2, GL_SPECULAR, green);
    glLightfv(GL_LIGHT3, GL_SPECULAR, blue);

    glGenTextures(2, textures);

    glPixelStorei(GL_UNPACK_ALIGNMENT, 4);
    glBindTexture(GL_TEXTURE_2D, textures[0]);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);

    glTexImage2D(GL_TEXTURE_2D, 0, tex2.bytes_per_pixel, tex1.width, tex1.height, 0, GL_BGR_EXT, GL_UNSIGNED_BYTE, tex1.pixel_data);


    glBindTexture(GL_TEXTURE_2D, textures[1]);
    glTexImage2D(GL_TEXTURE_2D, 0, tex2.bytes_per_pixel, tex2.width, tex2.height, 0, GL_BGR_EXT, GL_UNSIGNED_BYTE, tex2.pixel_data);

    // enter GLUT event processing cycle
    glutMainLoop();

    return 1;
}