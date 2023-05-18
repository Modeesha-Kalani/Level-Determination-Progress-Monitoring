import json

from flask import Flask, request
import pickle as pickles

app = Flask(__name__)


def Predict_Level(Total_Points, Time_Spent):
    open_file = open("Model_Data.pkl", "rb")
    object_list = pickles.load(open_file)
    scaler = object_list[0]
    model = object_list[1]
    open_file.close()

    X = [[Total_Points, Time_Spent]]
    X = scaler.transform(X)
    pred = model.predict(X)
    Level = int(pred[0])
    return Level


@app.route("/healthCheck")
def hello_world():
    return "<p>Hello, Welcome</p>"

# provide
# http://127.0.0.1:5000/predictLevel?TotalPoints=10&time=10
@app.route("/predictLevel")
def predictHealthCall():
    point = request.args.get('TotalPoints')
    time = request.args.get('time')
    response = Predict_Level(point, time)
    print(response)
    response -= 1
    json_string = json.dumps({'level': response})
    return json_string


if __name__ == '__main__':
    app.run(debug=True)
