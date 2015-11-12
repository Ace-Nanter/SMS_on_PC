#include <stdio.h>
#include <libusb.h>
#include <string.h>

#define ENDPOINT_BULK_IN 0x83
#define ENDPOINT_BULK_OUT 0x03 // Have tried 0x00, 0x01 and 0x02

#define VID 0x18D1
#define PID 0x4E11

#define ACCESSORY_PID 0x2D00
#define ACCESSORY_ADB_PID 0x2D01 // Can't get this to work, if ADB is active, can't get handle on device

/*
ON OSX
gcc adktest.c -I/usr/local/include -o adktest -lusb-1.0.0 -I/usr/local/include -I/usr/local/include/libusb-1.0
ON UBUNTU
gcc adktest.c -I/usr/include -o adktest -lusb-1.0 -I/usr/include -I/usr/include/libusb-1.0

Testing on Nexus One with Gingerbread 2.3.4
*/

static int transferTest();
static int init(void);
static int shutdown(void);
static void error(int code);
static void status(int code);
static int setupAccessory(
const char* manufacturer,
const char* modelName,
const char* description,
const char* version,
const char* uri,
const char* serialNumber);

//static
static struct libusb_device_handle* handle;

int main (int argc, char *argv[]){
if(init() < 0)
    return;

if(setupAccessory(
    "PCHost",
    "PCHost1",
    "Description",
    "1.0",
    "http://www.mycompany.com",
    "SerialNumber") < 0){
    fprintf(stdout, "Error setting up accessory\n");
    shutdown();
    return -1;
};
if(transferTest() < 0){
    fprintf(stdout, "Error in transferTest\n");
    shutdown();
    return -1;
}   
shutdown();
fprintf(stdout, "Finished\n");
return 0;
}

static int transferTest(){
  // TEST BULK IN/OUT
  const static int PACKET_BULK_LEN=64;
  const static int TIMEOUT=5000;
  int r,i;
  int transferred;
  char answer[PACKET_BULK_LEN];
  char question[PACKET_BULK_LEN];
  for (i=0;i<PACKET_BULK_LEN; i++) question[i]=i;

    // ***FAILS HERE***
    r = libusb_bulk_transfer(handle, ENDPOINT_BULK_OUT, question, PACKET_BULK_LEN,
                             &transferred,TIMEOUT);
    if (r < 0) {
        fprintf(stderr, "Bulk write error %d\n", r);
        error(r);
        return r;
    }
    fprintf(stdout, "Wrote %d bytes", r);

    r = libusb_bulk_transfer(handle, ENDPOINT_BULK_IN, answer,PACKET_BULK_LEN,
                             &transferred, TIMEOUT);
    if (r < 0) {
        fprintf(stderr, "Bulk read error %d\n", r);
        error(r);
        return r;
    }
    fprintf(stdout, "Read %d bytes", r);

    if (transferred < PACKET_BULK_LEN) {
        fprintf(stderr, "Bulk transfer short read (%d)\n", r);
        error(r);
        return -1;
    }
    printf("Bulk Transfer Loop Test Result:\n");
    //     for (i=0;i< PACKET_BULK_LEN;i++) printf("%i, %i,\n ",question[i],answer[i]);
    for(i = 0;i < PACKET_BULK_LEN; i++) {
        if(i%8 == 0)
            printf("\n");
        printf("%02x, %02x; ",question[i],answer[i]);
    }
    printf("\n\n");

    return 0;

}


static int init(){
libusb_init(NULL);
if((handle = libusb_open_device_with_vid_pid(NULL, VID, PID)) == NULL){
    fprintf(stdout, "Problem acquiring handle\n");
    return -1;
}
libusb_claim_interface(handle, 0);  

return 0;
}

static int shutdown(){
if(handle != NULL)
    libusb_release_interface (handle, 0);
libusb_exit(NULL);
return 0;
}

static int setupAccessory(
const char* manufacturer,
const char* modelName,
const char* description,
const char* version,
const char* uri,
const char* serialNumber){

unsigned char ioBuffer[2];
int devVersion;
int response;

response = libusb_control_transfer(
    handle, //handle
    0xC0, //bmRequestType
    51, //bRequest
    0, //wValue
    0, //wIndex
    ioBuffer, //data
    2, //wLength
    0 //timeout
);

if(response < 0){error(response);return-1;}

devVersion = ioBuffer[1] << 8 | ioBuffer[0];
fprintf(stdout,"Version Code Device: %d\n", devVersion);

usleep(1000);//sometimes hangs on the next transfer :(

response = libusb_control_transfer(handle,0x40,52,0,0,(char*)manufacturer,strlen(manufacturer),0);
if(response < 0){error(response);return -1;}
response = libusb_control_transfer(handle,0x40,52,0,1,(char*)modelName,strlen(modelName)+1,0);
if(response < 0){error(response);return -1;}
response = libusb_control_transfer(handle,0x40,52,0,2,(char*)description,strlen(description)+1,0);
if(response < 0){error(response);return -1;}
response = libusb_control_transfer(handle,0x40,52,0,3,(char*)version,strlen(version)+1,0);
if(response < 0){error(response);return -1;}
response = libusb_control_transfer(handle,0x40,52,0,4,(char*)uri,strlen(uri)+1,0);
if(response < 0){error(response);return -1;}
response = libusb_control_transfer(handle,0x40,52,0,5,(char*)serialNumber,strlen(serialNumber)+1,0);
if(response < 0){error(response);return -1;}

fprintf(stdout,"Accessory Identification sent\n", devVersion);

response = libusb_control_transfer(handle,0x40,53,0,0,NULL,0,0);
if(response < 0){error(response);return -1;}

fprintf(stdout,"Attempted to put device into accessory mode\n", devVersion);

if(handle != NULL)
    libusb_release_interface (handle, 0);

int tries = 4;
for(;;){
    tries--;
    if((handle = libusb_open_device_with_vid_pid(NULL, VID, ACCESSORY_PID)) == NULL){
        if(tries < 0){
            return -1;
        }
    }else{
        break;
    }
    sleep(1);
}

libusb_claim_interface(handle, 0);
fprintf(stdout, "Interface claimed, ready to transfer data\n");
return 0;
}

// error reporting function left out for brevity