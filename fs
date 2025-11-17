#include <sys/socket.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <dirent.h>

struct sockaddr_in serv_addr, cli_addr;
int listenfd, connfd, r, w, cli_addr_len;
unsigned short serv_port = 25020; /* port number */
char serv_ip[] = "192.168.24.15"; /* listen on all interfaces */
char buffer[1024]; /* buffer for file chunks */

int main() {
    FILE *fp;
    char filename[256], prefix[2];  // 1 character + null-terminator
    struct dirent *entry;
    DIR *dp;
    char file_list[1024] = "";  // String to store list of matching files
    char *msg;

    bzero(&serv_addr, sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_port = htons(serv_port);
    inet_aton(serv_ip, &serv_addr.sin_addr);

    printf("\nTCP ITERATIVE FILE TRANSFER SERVER.\n");

    if ((listenfd = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
        perror("SERVER ERROR: Cannot create socket");
        exit(1);
    }

    if (bind(listenfd, (struct sockaddr*)&serv_addr, sizeof(serv_addr)) < 0) {
        perror("SERVER ERROR: Cannot bind");
        close(listenfd);
        exit(1);
    }

    if (listen(listenfd, 5) < 0) {
        perror("SERVER ERROR: Cannot listen");
        close(listenfd);
        exit(1);
    }

    cli_addr_len = sizeof(cli_addr);

    for (;;) {
        printf("\nSERVER: Waiting for client connection...\n");

        if ((connfd = accept(listenfd, (struct sockaddr*)&cli_addr, (socklen_t*)&cli_addr_len)) < 0) {
            perror("SERVER ERROR: Cannot accept client");
            close(listenfd);
            exit(1);
        }

        printf("SERVER: Connected to client %s\n", inet_ntoa(cli_addr.sin_addr));

        /* Receive the first letter or prefix from the client */
        bzero(prefix, sizeof(prefix));
        r = read(connfd, prefix, sizeof(prefix) - 1);
        if (r <= 0) {
            perror("SERVER ERROR: Cannot read prefix");
            close(connfd);
            continue;
        }
        prefix[r] = '\0';  // Null-terminate string

        printf("SERVER: Client requested files starting with '%s'\n", prefix);

        /* List files starting with the provided prefix */
        dp = opendir("./");  // Assuming the server's working directory contains files
        if (dp == NULL) {
            perror("SERVER ERROR: Cannot open directory");
            close(connfd);
            continue;
        }

        bzero(file_list, sizeof(file_list));
        while ((entry = readdir(dp)) != NULL) {
            // Check if the file starts with the prefix
            if (strncmp(entry->d_name, prefix, strlen(prefix)) == 0) {
                strcat(file_list, entry->d_name);
                strcat(file_list, "\n");
            }
        }
        closedir(dp);

        if (strlen(file_list) == 0) {
            msg = "No files found with the specified prefix.\n";
            write(connfd, msg, strlen(msg));
            close(connfd);
            printf("SERVER: No files found with prefix '%s', connection closed.\n", prefix);
            continue;
        }

        /* Send list of files to client */
        printf("SERVER: Sending list of files starting with '%s':\n%s\n", prefix, file_list);
        write(connfd, file_list, strlen(file_list));

        /* Receive the filename chosen by the client */
        bzero(filename, sizeof(filename));
        r = read(connfd, filename, sizeof(filename) - 1);
        if (r <= 0) {
            perror("SERVER ERROR: Cannot read filename");
            close(connfd);
            continue;
        }
        filename[r] = '\0';  // Null-terminate string

        printf("SERVER: Client requested file: %s\n", filename);

        /* Open requested file */
        fp = fopen(filename, "rb");
        if (fp == NULL) {
            msg = "ERROR: File not found\n";
            write(connfd, msg, strlen(msg));
            close(connfd);
            printf("SERVER: File not found, connection closed.\n");
            continue;
        }

        printf("SERVER: Sending file...\n");

        /* Send file data */
        while ((r = fread(buffer, 1, sizeof(buffer), fp)) > 0) {
            w = write(connfd, buffer, r);
            if (w < 0) {
                perror("SERVER ERROR: Write to socket failed");
                break;
            }
        }

        fclose(fp);
        close(connfd);

        printf("SERVER: File sent and connection closed.\n");
    }

    close(listenfd);
    return 0;
}
