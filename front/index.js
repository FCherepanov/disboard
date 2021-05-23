import "ol/ol.css";
import GeoJSON from "ol/format/GeoJSON";
import Map from "ol/Map";
import View from "ol/View";
import { Circle as CircleStyle, Fill, Stroke, Style } from "ol/style";
import { OSM, Vector as VectorSource } from "ol/source";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";

import moment from 'moment'
import 'moment/locale/ru';
moment.locale('ru');
/** @type {VectorSource<import("../src/ol/geom/SimpleGeometry.js").default>} */
var source = new VectorSource({
  url: "data/admin_level_4.json",
  format: new GeoJSON()
});
var style = new Style({
  fill: new Fill({
    color: "rgba(120, 120, 120, 0.2)"
  }),
  stroke: new Stroke({
    color: "#319FD3",
    width: 1
  }),
  image: new CircleStyle({
    radius: 5,
    fill: new Fill({
      color: "rgba(255, 0, 0, 1)"
    }),
    stroke: new Stroke({
      color: "#319FD3",
      width: 1
    })
  })
});

function getDisColor(disLevel, alpha)
{
  const rLevel = disLevel * 2.55;
  const glevel = 255 - rLevel;
  return `rgba(${rLevel}, ${glevel}, 0, ${alpha})`;
}

function getDisStyle(disLevel) {
    const res = new Style({
    fill: new Fill({
      color: getDisColor(disLevel, 0.4)
    }),
    stroke: new Stroke({
      color: "#319FD3",
      width: 1
    })
  });
  return res;
}

var vectorLayer = new VectorLayer({
  source: source,
  style: style,
});

var view = new View({
  center: [11766638.633803273, 10782690.646556962],
  zoom: 3.5
});

var map = new Map({
  layers: [
    new TileLayer({
      source: new OSM()
    }),
    vectorLayer
  ],
  target: "map",
  view: view
});

function resetView()
{
  view.setZoom(3.5);
  view.setCenter( [11766638.633803273, 10782690.646556962]);
}

function setRegionDisLevel(regionId, disLevel) {
  var feature = source.getFeatureById(regionId);
  feature.setStyle(getDisStyle(disLevel));
  return feature;
}

function removeAllChildNodes(parent) {
  if ('content' in document.createElement('template')) {
    while (parent.firstChild) {
      parent.removeChild(parent.firstChild);
    }
  }
}

function addNotify(eventlist, disType, regionName, regionId, disValue) 
{
  if ('content' in document.createElement('template')) {

    const template = document.querySelector('#event');
    const clone = template.content.cloneNode(true);
    clone.querySelector(".dis_type").innerHTML = disType;
    clone.querySelector(".percent").innerHTML = disValue+"%";
    clone.querySelector(".region_name").innerHTML = regionName;
    clone.querySelector("a").setAttribute("regionId", regionId);
    clone.querySelector(".captions").style.color = getDisColor(disValue, 1.0);
    eventlist.appendChild(clone);
  }
}

function parseData(jdata) {
  const eventlist = document.querySelector("#eventlist");
  removeAllChildNodes(eventlist);
  resetView();
  for (var i = 0; i < jdata.length; i++) {
    var obj = jdata[i];
    const ft = setRegionDisLevel(obj.regionId, obj.value);
    if (obj.value > 50)
    {
      addNotify(eventlist, obj.caption,  ft.get("name"), obj.regionId, obj.value);
    }
  }
}


function getDisPrediction() {
  const period = document.getElementById("periodSlider").value;
  document.getElementById("current").innerHTML = moment().clone().startOf('day').add(period, 'days').calendar().split(", в")[0];
  fetch("/DisLevel?dt=" + period, {
  //fetch("http://localhost:5000/DisLevel?dt=" + period, {
    method: "get",
  })
    .then((response) => response.json())
    .then((jsonData) => parseData(jsonData))
    .catch((err) => {
      console.error(err);
    });
}

function selectReg(event) {
    var feature = source.getFeatureById(event.getAttribute("regionId"));
  var polygon = feature.getGeometry();
  view.fit(polygon);
}

window.selectReg = selectReg;

var periodSlider = document.getElementById("periodSlider");
periodSlider.addEventListener(
  "input",
  getDisPrediction,
  false
);
const eventlist = document.querySelector("#eventlist");
removeAllChildNodes(eventlist);
setTimeout(getDisPrediction, 10000);  // Временное явление, ждем пока прогрузятся границы субъектов.