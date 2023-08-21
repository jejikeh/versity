package main

import (
	"flag"
	"fmt"
	"log"
	"os"
	"os/exec"
	"strings"
	"unicode"
)

func main() {
	servicesPtr := flag.String("services", "api-gateway-service,sessions-service", "names of kubernetes services")
	searchesPtr := flag.String("searches", "API_URL,SESSIONS_URL", "content of url line")
	filesPtr := flag.String(
		"files",
		"src/versity-frontend-react/src/https/index.ts,src/versity-frontend-react/src/services/SessionConnectionService.ts",
		"file paths")
	urlPtr := flag.String("url", "/api,/sessions-hub", "url content")

	flag.Parse()

	servicesNames := strings.Split(*servicesPtr, ",")
	searchesStrings := strings.Split(*searchesPtr, ",")
	filesPaths := strings.Split(*filesPtr, ",")
	urlPaths := strings.Split(*urlPtr, ",")

	for i := range servicesNames {
		ingressHost, port := getHostAndPortFromKubectl(servicesNames[i])

		replaceLineInFile(
			filesPaths[i],
			"export const "+searchesStrings[i],
			fmt.Sprintf("%s = \"http://%s:%s%s\"",
				"export const "+searchesStrings[i],
				ingressHost,
				port,
				urlPaths[i]))
	}
}

func getHostAndPortFromKubectl(serviceName string) (string, string) {
	log.Println("Exec: kubectl get svc")
	execGetSvc := exec.Command("kubectl", "get", "svc")

	allServicesByte, err := execGetSvc.Output()

	if err != nil {
		log.Fatal(err)
	}

	allServices := string(allServicesByte)
	log.Print(string(allServices))

	log.Printf("looking for %s", serviceName)

	if !strings.Contains(allServices, serviceName) {
		log.Fatalf("%s was not found in services", serviceName)
	}

	log.Printf("Exec: kubectl describe svc %s", serviceName)
	execDescribeService := exec.Command("kubectl", "describe", "svc", serviceName)

	serviceDescriptionByte, err := execDescribeService.Output()

	if err != nil {
		log.Fatal(err)
	}

	serviceDescription := string(serviceDescriptionByte)
	log.Print(serviceDescription)

	mapDescription := strings.Split(serviceDescription, ":")
	ingressHost := ""
	port := ""

	for i, v := range mapDescription {
		if strings.Contains(v, "LoadBalancer Ingress") {
			ingressHost = strings.TrimSpace(strings.Split(mapDescription[i+1], "\n")[0])
			continue
		}

		if strings.Contains(v, "Port") {
			tempPort := strings.TrimSpace(strings.Split(mapDescription[i+1], "\n")[0])
			for _, v := range tempPort {
				if unicode.IsDigit(v) {
					port = port + string(v)
				}
			}
			break
		}
	}

	log.Printf("INGRESS_HOST: %s", ingressHost)
	log.Printf("Port: %s", port)

	return ingressHost, port
}

func replaceLineInFile(path string, prefix string, content string) {
	inputIndexBytes, err := os.ReadFile(path)

	if err != nil {
		log.Fatalln(err)
	}

	inputIndexLines := strings.Split(string(inputIndexBytes), "\n")

	for i, line := range inputIndexLines {
		if strings.Contains(line, prefix) {
			inputIndexLines[i] = content
		}
	}

	outputIndexLines := strings.Join(inputIndexLines, "\n")
	err = os.WriteFile(path, []byte(outputIndexLines), 0644)
	if err != nil {
		log.Fatalln(err)
	}
}
