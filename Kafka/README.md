# Kafka Modules for Crosser Edge

To run the unit tests you have to setup kafka locally or you can create a free kafkacluster at https://customer.cloudkarafka.com/instance/create?plan=ducky

PreReq: docker and docker-compose are installed

 1. Clone the confluentinc/cp-all-in-one GitHub repository and check out the 5.5.0-post branch.
 ```
 git clone https://github.com/confluentinc/cp-all-in-one
 cd cp-all-in-one
 git checkout 5.5.0-post
 ```

 2. Navigate to /cp-all-in-one/cp-all-in-one directory.
 ```
 cd cp-all-in-one/
 ```

 3. docker-compose up -d
 ```
 The last step will take a while
 ```

If you get issues, verify that your docker is allwoed to take more than 2GB se deailt in article https://docs.confluent.io/5.5.0/quickstart/ce-docker-quickstart.html