package main

import (
	"log"
	"os/exec"
	"strings"
	"unicode"
)

var ServiceName = "api-gateway-service"

func main() {
	log.Println("Exec: kubectl get svc")
	execGetSvc := exec.Command("kubectl", "get", "svc")

	allServicesByte, err := execGetSvc.Output()

	if err != nil {
		log.Fatal(err)
	}

	allServices := string(allServicesByte)
	log.Print(string(allServices))

	log.Printf("looking for %s", ServiceName)

	if !strings.Contains(allServices, ServiceName) {
		log.Fatalf("%s was not found in services", ServiceName)
	}

	log.Printf("Exec: kubectl describe svc %s", ServiceName)
	execDescribeService := exec.Command("kubectl", "describe", "svc", ServiceName)

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
}
