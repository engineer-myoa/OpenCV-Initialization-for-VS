/*
 Initializer developed by [Kongju National University 201301966 Shin Woo-Jin]
 If you have any questions, please send me an email.
 myoatm@gmail.com

 Version 1.3.0
 
*/

#include <iostream>
#include <opencv2\opencv.hpp>

using namespace std;
using namespace cv;

int main() {

	Mat src = imread("lena.png", IMREAD_GRAYSCALE);
	if (src.empty()) {
		return -1;
	}
	imshow("title", src);
	waitKey(0);

	return 0;
}