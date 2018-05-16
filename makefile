build:
	g++ -o kupa -std=c++14 -lglfw *.cpp common/*.cpp -lglut -lGLEW -lGLU -lGL -lglfw3 -lX11 -lXi -lXrandr -lXxf86vm -lXinerama -lXcursor -ldl -lrt -lm -pthread -g
